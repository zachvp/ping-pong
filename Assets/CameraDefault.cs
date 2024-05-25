using UnityEngine;

public class CameraDefault : MonoBehaviour
{
    public GameObject target;

    void Update()
    {
        transform.LookAt(target.transform);
    }
}
