using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkPlayer : NetworkBehaviour
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

    private void Start()
    {
        Init();

        if (IsOwner || IsServer)
        {
            //UIDebug.Instance.Register($"ClientID {OwnerClientId} Local Velocity", () => body.velocity);
            //UIDebug.Instance.Register($"ClientID {OwnerClientId} Network Velocity", () => velocity.Value);
        }
    }

    private void FixedUpdate()
    {
        if (IsOwner)
        {
            body.velocity = character.Velocity;
            UpdateServerRPC(character.Velocity, character.state, character.buffer);
        }
    }

    [ServerRpc]
    public void UpdateServerRPC(
        Vector3 newVelocity,
        PlayerCharacter.State newState,
        PlayerCharacter.State newBuffer)
    {
        velocity.Value = newVelocity;
        state.Value = newState;
        buffer.Value = newBuffer;
    }

    private void Init()
    {
        Debug.Log($"set spawn and camera position");

        if (IsOwner)
        {
            // todo: hacks
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
        }
    }
}
