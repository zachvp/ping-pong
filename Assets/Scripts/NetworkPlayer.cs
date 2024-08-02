using Unity.Netcode;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    private Rigidbody body;

    public NetworkVariable<Vector3> velocity = new();

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

    private void Update()
    {
        if (IsServer)
        {
            body.velocity = velocity.Value;
            //transform.position = body.position;
        }
    }

    [ServerRpc]
    public void UpdateServerRPC(Vector3 newVelocity)
    {
        velocity.Value = newVelocity;
    }
}
