using UnityEngine;

public class OffsetTransform : MonoBehaviour
{
    public Transform source;
    public Vector3 offset;
    public SharedVector3 direction;

    public void Awake()
    {
        direction.vector3.onChanged += (oldValue, newValue) =>
        {
            var resolvedOffset = offset;

            transform.position = source.position + Vector3.Scale(offset, newValue);
        };
    }
}
