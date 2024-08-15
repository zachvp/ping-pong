using Unity.Burst;
using Unity.Netcode;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour, INetworkGameStateHandler
{
    public GameObject ownerPrefab;
    public GameObject ownerRoot;

    private Rigidbody body;

    public NetworkPlayerSharedState sharedState;

    public Vector3 positionSpawn = new Vector3(0, 5, -1f);

    private PlayerCharacter character;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Init();

        HostGameState.Instance.RegisterGameResetHandler(this);
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            UIDebug.Instance.Register($"ClientID", $"{OwnerClientId}");
            //var owner = Instantiate(ownerPrefab, ownerRoot.transform);
            character = ownerRoot.GetComponentInChildren<PlayerCharacter>();
        }
        else
        {
            ownerRoot.SetActive(false);
        }
        //UIDebug.Instance.Register($"ClientID {OwnerClientId} Spawned", $"ObjectId: {NetworkObjectId}, Host: {IsHost}, Owner: {IsOwner}, Client: {IsClient}");
    }

    private void FixedUpdate()
    {
        if (IsOwner)
        {
            body.velocity = character.Velocity;
            sharedState.UpdateServerRPC(character.Velocity, character.state, character.buffer);
        }
    }

    private void Init()
    {
        if (IsOwner)
        {
            StartCoroutine(Task.FixedUpdate(() =>
            {
                body.position = HostGameState.Instance.spawns[OwnerClientId].position;
                StartCoroutine(Task.FixedUpdate(() => body.constraints |= RigidbodyConstraints.FreezePositionZ));
            }));
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void HandleGameStateChangeRpc(GameState old, GameState current)
    {
        if (IsOwner)
        {
            switch (current)
            {
                case GameState.MAIN:
                    break;
                case GameState.RESET:
                    Debug.Log($"{OwnerClientId} handle game reset");

                    StartCoroutine(Task.FixedUpdate(() =>
                    {
                        body.constraints &= ~RigidbodyConstraints.FreezePositionZ;
                        body.position = HostGameState.Instance.spawns[OwnerClientId].position;
                        body.velocity = Vector3.zero;

                        StartCoroutine(Task.FixedUpdate(() => body.constraints |= RigidbodyConstraints.FreezePositionZ));
                    }));
                    break;
            }
        }
    }
}
