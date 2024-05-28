using UnityEngine.InputSystem;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public Vector3 initialVelocity;

    public float spinMultiplier = 1;
    public Vector2 spinDampening = Vector2.one;
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

    private void OnCollisionEnter(Collision collision)
    {
        var newVelocity = body.velocity;
        var playerCharacter = collision.gameObject.GetComponent<PlayerInputMapper>();

        Debug.Log($"collision contacts: {collision.contacts.Length}");

        foreach (var c in collision.contacts)
        {
            var contactNormalDotBack = Vector3.Dot(c.normal, Vector3.back); // body.velocity.normalized
            var contactNormalDotUp = Vector3.Dot(c.normal, Vector3.up);
            var contactNormalDotRight = Vector3.Dot(c.normal, Vector3.right);

            if (Mathf.Abs(contactNormalDotBack) > 0.9f)
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
                    //Debug.Log($"player velocity: {playerVelocity}");

                    if (playerCharacter.state == PlayerInputMapper.State.HIT)
                    {
                        //Debug.Log($"apply hit boost");
                        newVelocity.z *= hitMultiplier;
                        newVelocity.z = Mathf.Clamp(newVelocity.z, -hitMultiplier * initialVelocity.z, hitMultiplier * initialVelocity.z);

                        var spinX = playerVelocity.x * spinMultiplier * hitMultiplier * spinDampening.x;
                        var spinY = playerVelocity.y * spinMultiplier * hitMultiplier * spinDampening.y;
                        //Debug.Log($"spinX: {spinX}, spinY: {spinY}");

                        newVelocity.x -= spinX;
                        newVelocity.y -= spinY;
                    }
                    else
                    {
                        newVelocity.x -= playerVelocity.x * spinMultiplier * spinDampening.x;
                        newVelocity.y -= playerVelocity.y * spinMultiplier * spinDampening.y;
                    }
                }    
            }

            if (Mathf.Abs(contactNormalDotUp) > 0.6f)
            {
                Debug.LogFormat($"contactNormalDotUp: {contactNormalDotUp}");
                newVelocity.y *= 0.6f;
                newVelocity.z = initialVelocity.z * Utils.SignMultiplier(newVelocity.z);
            }

            if (Mathf.Abs(contactNormalDotRight) > 0.6f)
            {
                Debug.LogFormat($"contactNormalDotUp: {contactNormalDotUp}");
                newVelocity.x *= 0.6f;
                newVelocity.z = initialVelocity.z * Utils.SignMultiplier(newVelocity.z);
            }
        }

        body.velocity = newVelocity;
    }
}
