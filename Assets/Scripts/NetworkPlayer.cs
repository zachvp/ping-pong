using Unity.Netcode;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    private Rigidbody body;

    public NetworkVariable<Vector3> velocity = new();
    public Vector3 positionSpawn = new Vector3(0, 5, -1f);

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
            UIDebug.Instance.Register($"ClientID", $"{OwnerClientId}");
        //UIDebug.Instance.Register($"ClientID {OwnerClientId} Spawned", $"ObjectId: {NetworkObjectId}, Host: {IsHost}, Owner: {IsOwner}, Client: {IsClient}");
    }

    private void Start()
    {
        if (IsOwner)
        {
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

        if (IsServer)
        {
            StartCoroutine(Task.FixedUpdate(() => body.position = HostGameState.Instance.spawns[OwnerClientId].position));
        }
    }

    private void Update()
    {
        if (IsServer)
        {
            body.velocity = velocity.Value;
        }
    }

    [ServerRpc]
    public void UpdateServerRPC(Vector3 newVelocity)
    {
        velocity.Value = newVelocity;
    }
}
