using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class TestNetworkPlayer : NetworkBehaviour
{
    public NetworkVariable<Vector3> position = new();

    public Vector3 startPosition = new Vector3(0, 5, -1);
    public float speed;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
            UIDebug.Instance.Register($"ClientID", $"{OwnerClientId}");
        //UIDebug.Instance.Register($"ClientID {OwnerClientId} Spawned", $"ObjectId: {NetworkObjectId}, Host: {IsHost}, Owner: {IsOwner}, Client: {IsClient}");
    }

    private void Start()
    {
        transform.position = startPosition;
    }

    private void Update()
    {
        if (IsServer)
        {
            transform.position += position.Value * Time.deltaTime;
        }

        if (IsClient && IsOwner)
        {
            var newPosition = PositionFromInput(Keyboard.current);

            UpdateServerRPC(newPosition);
        }
    }

    [ServerRpc]
    public void UpdateServerRPC(Vector3 newPosition)
    {
        position.Value = newPosition;
    }

    private Vector3 PositionFromInput(Keyboard keyboard)
    {
        var direction = Vector3.zero;

        if (keyboard.dKey.isPressed)
        {
            direction += Vector3.right;
        }
        if (keyboard.aKey.isPressed)
        {
            direction += Vector3.left;
        }

        if (keyboard.wKey.isPressed)
        {
            direction += Vector3.up;
        }
        if (keyboard.sKey.isPressed)
        {
            direction += Vector3.down;
        }

        return (speed * direction);
    }
}
