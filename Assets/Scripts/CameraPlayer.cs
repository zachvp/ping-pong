using UnityEngine;

public class CameraPlayer : MonoBehaviour
{
    public Vector3 offsetFromTarget;

    public PlayerSharedState sharedState;

    private void Update()
    {
        sharedState.cameraForwardZ = (int) transform.forward.z;
    }
}
