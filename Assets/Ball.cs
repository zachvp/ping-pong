using UnityEngine.InputSystem;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public Vector3 initialVelocity;
    public float spinMultiplier = 1;

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

    private void OnCollisionEnter(Collision collision)
    {
        var newVelocity = body.velocity;
        var playerCharacter = collision.gameObject.GetComponent<PlayerInputMapper>();
        if (playerCharacter)
        {
            var playerVelocity = playerCharacter.FrameVelocity;
            Debug.Log($"player velocity: {playerVelocity}");
            newVelocity.x -= playerVelocity.x * spinMultiplier;
            newVelocity.y -= playerVelocity.y * spinMultiplier;
        }
        //if (collision.cont)
        foreach (var c in collision.contacts)
        {
            var dot = Vector3.Dot(c.normal, body.velocity.normalized);
            Debug.Log($"dot: {dot}");
            if (dot > 0.9f)
            {
                Debug.Log($"apply bounce boost");
                
                newVelocity.z = newVelocity.z > 0 ? initialVelocity.z : -initialVelocity.z;
                body.velocity = newVelocity;
                break;
            }
            
        }
    }
}
