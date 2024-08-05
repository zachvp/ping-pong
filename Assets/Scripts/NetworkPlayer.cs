using Unity.Netcode;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    public GameObject ownerPrefab;
    public GameObject ownerRoot;

    private Rigidbody body;

    public NetworkVariable<Vector3> velocity = new();
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
        }

        //if (IsServer)
        if (IsOwner)
        {
            StartCoroutine(Task.FixedUpdate(() => body.position = HostGameState.Instance.spawns[OwnerClientId].position));
        }
    }

    private void Update()
    {
        if (IsOwner)
            body.velocity = character.Velocity;

        //if (IsServer)
        //{
        //    body.velocity = velocity.Value;
        //}
    }

    private void FixedUpdate()
    {
        if (IsOwner)
        {
            //UpdateServerRPC(character.Velocity);
        }
    }

    [ServerRpc]
    public void UpdateServerRPC(Vector3 newVelocity)
    {
        velocity.Value = newVelocity;
    }
}
