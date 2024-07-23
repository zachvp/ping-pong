using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

public class NetworkPlayer : NetworkBehaviour
{
    public NetworkVariable<Vector3> networkPosition = new();
    public NetworkRigidbody networkRigidbody;

    public override void OnNetworkSpawn()
    {
        networkRigidbody = GetComponent<NetworkRigidbody>();

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
        //networkPosition.Value = GetComponent<Rigidbody>().position;
        //networkRigidbody.v
    }
}
