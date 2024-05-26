using UnityEngine.InputSystem;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public Vector3 initialVelocity;
    private Vector3 initialPosition;
    private Rigidbody body;

    private void Awake() {
        body = GetComponent<Rigidbody>();
        initialPosition = transform.position;
    }

    private void Start() {
        body.velocity = initialVelocity;
    }

    private void Update() {
        if (Keyboard.current.backspaceKey.wasPressedThisFrame) {
            body.position = initialPosition;
            body.velocity = initialVelocity;
        }
    }
}
