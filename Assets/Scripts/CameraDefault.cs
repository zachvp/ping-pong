using UnityEngine;

public class CameraDefault : MonoBehaviour
{
    public Vector3 offsetFromTarget;

    private GameObject target;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player");
    }

    private void LateUpdate()
    {
        var newPosition = transform.position;
        newPosition.x = target.transform.position.x;
        newPosition.y = target.transform.position.y;
        newPosition += offsetFromTarget;

        transform.position = newPosition;
    }
}
