using UnityEngine;

public class OffsetTransform : MonoBehaviour
{
    public Transform source;
    public Collider sourceCollider;
    public Vector3 offset;
    public SharedVector3 direction;

    public void Start()
    {
        direction.vector3.onChanged += (oldValue, newValue) =>
        {
            // if move right, position at left edge
            var newPosition = source.position;
            if (newValue.x > 0)
            {
                newPosition.x = sourceCollider.bounds.min.x;
            }
            // if move left, position at right edge
            else if (newValue.x < 0)
            {
                newPosition.x = sourceCollider.bounds.max.x;
            }

            // if move up, position at bottom edge
            if (newValue.y > 0)
            {
                newPosition.y = sourceCollider.bounds.min.y;
            }
            // if move left, position at top edge
            else if (newValue.y < 0)
            {
                newPosition.y = sourceCollider.bounds.max.y;
            }

            transform.position = newPosition + offset;
        };
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 0.75f);
    }

    public void Update()
    {
        
        //Debug.DrawRay(leftEdge, Vector3.up * 4, Color.magenta);
    }
}
