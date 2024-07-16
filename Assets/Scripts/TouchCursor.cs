using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.Windows;

[RequireComponent (typeof(RectTransform))]
public class TouchCursor : MonoBehaviour
{
    public float joystickRadius = 64;
    public Transform anchor;

    private Vector3 initialPosition;
    private RectTransform rectTransform;

    public DebugValues debugValues;

    private void Awake()
    {
        initialPosition = transform.position;
        rectTransform = GetComponent<RectTransform>();
        //initialPosition = rectTransform.rect.position;
        debugValues.vector2_0 = initialPosition;
    }

    private void OnDrawGizmos()
    {
        var camera = GameObject.FindGameObjectWithTag(Constants.Tags.CAMERA).GetComponent<Camera>();
        Gizmos.DrawWireSphere(camera.ScreenToWorldPoint(initialPosition), 0.1f);
    }

    public Vector2 ComputeJoystickOffset(Vector2 screenPosition)
    {
        var fromTo = screenPosition - (Vector2)anchor.position;
        return Vector2.ClampMagnitude(fromTo, joystickRadius);
    }

    public Vector2 ComputeJoystickPosition(Vector2 offset)
    {
        return (Vector2) anchor.position + offset;
    }

    public void MoveCursor(Vector2 screenPosition)
    {
        transform.position = screenPosition;
    }

    public void ResetPosition()
    {
        transform.position = anchor.position;
    }
}
