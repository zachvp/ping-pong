using UnityEngine;

// todo: refactor to get target ref from hierarchy
public class CameraPlayer : MonoBehaviour
{
    public Vector3 offsetFromTarget;

    private GameObject target;

    private void Start()
    {
        target = GetComponentInParent<PlayerCharacter>().gameObject;
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
