using UnityEngine;
using Unity.Netcode;

public class NetworkPlayer : NetworkBehaviour
{
    public NetworkVariable<Vector3> velocity = new();
    public Rigidbody body;

    public override void OnNetworkSpawn()
    {
        Debug.Log($"{nameof(NetworkPlayer)} OnNetworkSpawn: ID: {NetworkObjectId}");
    }

    [ServerRpc]
    public void UpdateServerRPC(Vector3 newVelocity)
    {
        Debug.LogFormat($"server received network update for object: {NetworkObjectId}; ownerClient: {OwnerClientId}");
        velocity.Value = newVelocity;
    }
}
