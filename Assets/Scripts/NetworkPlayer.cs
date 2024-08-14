using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkPlayer : NetworkBehaviour, INetworkGameStateHandler
{
    public GameObject ownerPrefab;
    public GameObject ownerRoot;

    private Rigidbody body;

    public NetworkVariable<Vector3> velocity = new();
    public NetworkVariable<PlayerCharacter.State> state = new();
    public NetworkVariable<PlayerCharacter.State> buffer = new();

    public Vector3 positionSpawn = new Vector3(0, 5, -1f);

    private PlayerCharacter character;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Init();

        Debug.Log($"{OwnerClientId} Register game reset");
        HostGameState.Instance.RegisterGameResetHandler(this);
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            UIDebug.Instance.Register($"ClientID", $"{OwnerClientId}");
            var owner = Instantiate(ownerPrefab, ownerRoot.transform);
            character = owner.GetComponentInChildren<PlayerCharacter>();
        }
        //UIDebug.Instance.Register($"ClientID {OwnerClientId} Spawned", $"ObjectId: {NetworkObjectId}, Host: {IsHost}, Owner: {IsOwner}, Client: {IsClient}");
    }

    private void FixedUpdate()
    {
        if (IsOwner)
        {
            body.velocity = character.Velocity;
            UpdateServerRPC(character.Velocity, character.state, character.buffer);
        }
    }

    private void Init()
    {
        Debug.Log($"set spawn and camera position");

        if (IsOwner)
        {
            // todo: hacks; move to NetworkCamera
            // todo: rotate player character
            if (OwnerClientId > 0)
            {
                var camera = GetComponentInChildren<Camera>();

                var faceDirection = camera.transform.forward;
                faceDirection.z = -faceDirection.z;
                camera.transform.forward = faceDirection;

                var facePosition = camera.transform.position;
                facePosition.z = -facePosition.z;
                camera.transform.position = facePosition;
            }

            StartCoroutine(Task.FixedUpdate(() => body.position = HostGameState.Instance.spawns[OwnerClientId].position));
            StartCoroutine(Task.Delayed(Time.fixedDeltaTime * 2,
                () => StartCoroutine(Task.FixedUpdate(() => body.constraints |= RigidbodyConstraints.FreezePositionZ))));
        }
    }

    [Rpc(SendTo.Server)]
    public void UpdateServerRPC(
        Vector3 newVelocity,
        PlayerCharacter.State newState,
        PlayerCharacter.State newBuffer)
    {
        velocity.Value = newVelocity;
        state.Value = newState;
        buffer.Value = newBuffer;
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
