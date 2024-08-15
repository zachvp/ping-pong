using Unity.Netcode;
using UnityEngine;

public class NetworkPlayerSharedState : NetworkBehaviour
{
    public NetworkVariable<int> cameraForwardZ = new();
    public NetworkVariable<Vector3> velocity = new();
    public NetworkVariable<PlayerCharacter.State> state = new();
    public NetworkVariable<PlayerCharacter.State> buffer = new();

    [Rpc(SendTo.Server)]
    public void UpdateServerRPC(
    Vector3 newVelocity,
    PlayerCharacter.State newState,
    PlayerCharacter.State newBuffer)
    {
        velocity.Value = newVelocity;
        state.Value = newState;
        buffer.Value = newBuffer;
    }

    [Rpc(SendTo.Server)]
    public void UpdateServerOneshotRpc(int newCameraForwardZ) // todo: rename for 'camera'
    {
        cameraForwardZ.Value = newCameraForwardZ;
    }
}
