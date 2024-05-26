using UnityEngine;

public class CameraDefault : MonoBehaviour
{
    public GameObject target;
    public Vector3 offsetFromTarget;

    void LateUpdate()
    {
        var newPosition = transform.position;
        newPosition.x = target.transform.position.x;
        newPosition.y = target.transform.position.y;
        newPosition += offsetFromTarget;

        transform.position = newPosition;
        //transform.LookAt(target.transform);
    }
}
