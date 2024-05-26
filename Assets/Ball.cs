using UnityEngine.InputSystem;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public Vector3 initialVelocity;

    public float spinMultiplier = 1;
    public float hitMultiplier = 2;

    private Vector3 initialPosition;

    public Rigidbody Body { get { return body; } }
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

        foreach (var c in collision.contacts)
        {
            var dot = Vector3.Dot(c.normal, body.velocity.normalized);
            Debug.Log($"dot: {dot}");
            if (dot > 0.9f)
            {
                if (newVelocity.z > 0)
                {
                    newVelocity.z = Mathf.Max(initialVelocity.z, newVelocity.z);
                }
                else
                {
                    newVelocity.z = Mathf.Min(-initialVelocity.z, newVelocity.z);
                }
                if (playerCharacter)
                {
                    var playerVelocity = playerCharacter.FrameVelocity;
                    Debug.Log($"player velocity: {playerVelocity}");
                    newVelocity.x -= playerVelocity.x * spinMultiplier;
                    newVelocity.y -= playerVelocity.y * spinMultiplier;

                    if (playerCharacter.state == PlayerInputMapper.State.HIT)
                    {
                        Debug.Log($"apply hit boost");
                        newVelocity.z *= hitMultiplier;
                        newVelocity.z = Mathf.Clamp(newVelocity.z, -hitMultiplier * initialVelocity.z, hitMultiplier * initialVelocity.z);
                    }
                }
                
                break;
            }
        }

        if (playerCharacter)
        {
            //var playerVelocity = playerCharacter.FrameVelocity;
            //Debug.Log($"player velocity: {playerVelocity}");
            //newVelocity.x -= playerVelocity.x * spinMultiplier;
            //newVelocity.y -= playerVelocity.y * spinMultiplier;

            //if (playerCharacter.state == PlayerInputMapper.State.HIT)
            //{
            //    newVelocity.z = newVelocity.z > 0 ? initialVelocity.z * 1.5f : -initialVelocity.z * 1.5f;
            //}
        }

        body.velocity = newVelocity;
    }
}
