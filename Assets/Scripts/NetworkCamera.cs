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
            // todo: hacks; move to NetworkCamera
            // todo: rotate player character
            if (OwnerClientId > 0)
            {
                var camera = GetComponentInChildren<Camera>();

                var faceDirection = camera.transform.forward;
                faceDirection.z = -faceDirection.z;
                camera.transform.forward = faceDirection;
                sharedState.UpdateServerOneshotRpc((int)camera.transform.forward.z);

                var facePosition = camera.transform.position;
                facePosition.z = -facePosition.z;
                camera.transform.position = facePosition;
            }
        }
    }
}
