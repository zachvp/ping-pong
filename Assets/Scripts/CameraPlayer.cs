using UnityEngine;

// todo: remove useless class
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
        
    }
}
