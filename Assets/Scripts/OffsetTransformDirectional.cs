using UnityEngine;

public class OffsetTransform : MonoBehaviour
{
    public Vector3 offset;
    public Vector3 Direction;

    private Vector3 initialPosition;

    public void Awake()
    {
        initialPosition = transform.position;
        
        transform.position = initialPosition + Vector3.Scale(offset, Direction);
    }
}
