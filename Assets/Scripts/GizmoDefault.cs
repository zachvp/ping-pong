using UnityEngine;

public class GizmoDefault : MonoBehaviour
{
    public float radius = 0.25f;
    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
