using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

// todo: sync class and file name
public class OffsetTransform : MonoBehaviour
{
    public Transform source;
    public Collider sourceCollider;
    //public Vector3 offset;

    public void Start()
    {
        //direction.vector3.onChanged += (oldValue, newValue) =>
        //{
            
        //};
    }

    public void OnDrawGizmos()
    {
        //Gizmos.DrawWireSphere(transform.position, 0.75f);
    }

    public void Update()
    {
        
        //Debug.DrawRay(leftEdge, Vector3.up * 4, Color.magenta);
    }

    public void SetOffset(Vector3 direction)
    {
        //offset = newValue;

        // if move right, position at left edge
        var newPosition = source.position;
        if (direction.x > 0)
        {
            newPosition.x = sourceCollider.bounds.min.x;
        }
        // if move left, position at right edge
        else if (direction.x < 0)
        {
            newPosition.x = sourceCollider.bounds.max.x;
        }

        // if move up, position at bottom edge
        if (direction.y > 0)
        {
            newPosition.y = sourceCollider.bounds.min.y;
        }
        // if move left, position at top edge
        else if (direction.y < 0)
        {
            newPosition.y = sourceCollider.bounds.max.y;
        }

        transform.position = newPosition;
        //transform.position = newPosition + direction;
    }
}
