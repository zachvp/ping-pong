using Unity.Burst;
using Unity.Netcode;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour, INetworkGameStateHandler
{
    public GameObject ownerRoot;
    private Rigidbody body;
    public NetworkPlayerSharedState sharedState;
    public PlayerSharedState playerSharedState;

    private PlayerCharacter character;
    public VisualsPlayerCharacter visuals;

    public GameObject nonHostHacks;

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
        character = ownerRoot.GetComponentInChildren<PlayerCharacter>();

        if (IsOwner)
        {
            UIDebug.Instance.Register($"ClientID", $"{OwnerClientId}");
        }
        else
        {
            ownerRoot.SetActive(false);
        }

        if (IsHost && IsOwner)
        {
            nonHostHacks.SetActive(false);
        }

        //UIDebug.Instance.Register($"ClientID {OwnerClientId} Spawned", $"ObjectId: {NetworkObjectId}, Host: {IsHost}, Owner: {IsOwner}, Client: {IsClient}");
    }

    private void FixedUpdate()
    {
        if (IsOwner)
        {
            body.velocity = character.Velocity;
            sharedState.SetPlayerCharacterStateRPC(
                character.Velocity,
                character.state,
                character.buffer);
        }
    }

    private void LateUpdate()
    {
        if (sharedState.state.Value.HasFlag(PlayerCharacter.State.HIT))
        {
            // todo: move to event handler
            visuals.Flash(character.hitDurationFrames);
        }
        if (sharedState.state.Value.HasFlag(PlayerCharacter.State.DASH))
        {
            visuals.Trail(character.dashMoveFrames, sharedState.velocity.Value.normalized);
        }
    }

    private void Init()
    {
        if (IsOwner)
        {
            if (OwnerClientId > 0)
            {
                body.rotation = Quaternion.Euler(body.rotation.eulerAngles.x, body.rotation.eulerAngles.y + 180, body.rotation.eulerAngles.z);
                Debug.Log($"OwnerClientId set face direction: {body.rotation}");
                sharedState.SetCameraStateRpc(-1);
            }
            else
            {
                sharedState.SetCameraStateRpc(1);
            }

            StartCoroutine(Task.FixedUpdate(() =>
            {
                body.position = HostGameState.Instance.spawns[OwnerClientId].position;
                StartCoroutine(Task.FixedUpdate(() => body.constraints |= RigidbodyConstraints.FreezePositionZ));
                StartCoroutine(Task.FixedUpdate(() => body.constraints |= RigidbodyConstraints.FreezeRotationY));
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

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"collision: {collision.gameObject.name}");
        var ball = collision.gameObject.GetComponent<Ball>();

        if (ball && IsClient)
        {
            Debug.Log($"client-driven collision");
            ball.HandleCollision(new Vector3(0, 0, sharedState.cameraForwardZ.Value), playerSharedState, collision.gameObject.layer);
        }
    }
}
