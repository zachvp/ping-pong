using UnityEngine;
using Unity.Netcode;

public class RPCTest : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (!IsServer && IsOwner)
        {
            // todo:
            Debug.Log($"OnNetworkSpawn, test client RPC");
            TestClientRPC(0, NetworkObjectId);
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    void TestClientRPC(int value, ulong sourceNetworkObjectId)
    {
        Debug.Log($"Client received RPC #{value} on NetworkObject #{sourceNetworkObjectId}");
        if (IsOwner)
        {
            TestServerRPC(value + 1, sourceNetworkObjectId);
        }

    }

    [Rpc(SendTo.ClientsAndHost)]
    void TestServerRPC(int value, ulong sourceNetworkObjectId)
    {
        Debug.Log($"Server received the RPC #{value} on NetworkObject #{sourceNetworkObjectId}");
        TestClientRPC(value, sourceNetworkObjectId);
    }
}
