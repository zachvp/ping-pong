using Unity.Netcode;
using UnityEngine;

public class NetworkCamera : NetworkBehaviour
{
    public NetworkPlayerSharedState sharedState;

    private void Start()
    {
        Debug.Log($"set spawn and camera position");

        if (IsOwner)
        {
            if (OwnerClientId > 0)
            {
                var faceDirection = transform.forward;
                faceDirection.z = -faceDirection.z;
                transform.forward = faceDirection;
                sharedState.SetCameraStateRpc((int)transform.forward.z);

                var facePosition = transform.localPosition;
                facePosition.z = -facePosition.z;
                transform.localPosition = facePosition;
            }
        }
    }
}
