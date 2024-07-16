using UnityEngine;

[RequireComponent (typeof(RectTransform))]
public class TouchCursor : MonoBehaviour
{
    public float joystickRadius = 64;
    public Transform anchor;

    private Vector3 initialPosition;

    public DebugValues debugValues;

    private void Awake()
    {
        initialPosition = anchor.position;
        debugValues.vector2_0 = initialPosition;

        Debug.Log($"initialPosition: {initialPosition}\t anchor position: {anchor.position}");
    }

    private void OnEnable()
    {
        anchor.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        anchor.gameObject.SetActive(false);
    }

    public Vector2 JoystickRaw(Vector2 screenPosition)
    {
        var fromTo = screenPosition - (Vector2)anchor.position;
        return Vector2.ClampMagnitude(fromTo, joystickRadius);
    }

    public Vector2 JoystickScreenPosition(Vector2 offset)
    {
        return (Vector2) anchor.position + offset;
    }

    public Vector2 JoystickNormalized(Vector2 joystickRaw)
    {
        return joystickRaw / joystickRadius;
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
