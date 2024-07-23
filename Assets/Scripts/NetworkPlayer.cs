using UnityEngine;
using Unity.Netcode;

public class NetworkPlayer : NetworkBehaviour
{
    public NetworkVariable<Vector3> networkPosition = new();

    public override void OnNetworkSpawn()
    {
        Debug.Log($"{nameof(NetworkPlayer)} OnNetworkSpawn: ID: {NetworkObjectId}");

        if (IsOwner)
        {
            Debug.Log($"I is owner ({NetworkObjectId})");
        }
        if (IsClient)
        {
            Debug.Log($"I is client ({NetworkObjectId})");
        }

        // set start position
        if (NetworkObjectId < 2)
        {
            networkPosition.Value = new Vector3(0, 8, -1);
        }
        else
        {
            networkPosition.Value = new Vector3(0, 8, 23);
        }
    }

    private void Update()
    {
        transform.position = networkPosition.Value;
    }
}
