using UnityEngine;
using Unity.Netcode;

public class NetworkPlayer : NetworkBehaviour
{
    public NetworkVariable<Vector3> velocity = new();
    public Vector3 startPosition = new Vector3(0, 5, -1);

    public override void OnNetworkSpawn()
    {
        UIDebug.Instance.Register($"OwnerClientID {OwnerClientId} OnNetworkSpawn", $"NetworkObjectId: {NetworkObjectId} IsOwner: {IsOwner} IsHost: {IsHost} IsClient: {IsClient}");
        transform.position = startPosition;
        //Debug.Log($"{nameof(NetworkPlayer)} OnNetworkSpawn: OwnerClientID: {OwnerClientId} NetworkObjectId: {NetworkObjectId} Is");
    }

    //[ServerRpc]
    //public void UpdateServerRPC(Vector3 newVelocity)
    //{
    //    Debug.LogFormat($"server received network update for object: {NetworkObjectId}; ownerClient: {OwnerClientId}");
    //    velocity.Value = newVelocity;
    //}
}
