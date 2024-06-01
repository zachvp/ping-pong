using UnityEngine;

public class Ball : MonoBehaviour
{
    public Vector3 initialVelocity;

    public float spinMultiplier = 6;
    public float dashSpinMultiplier = 8;
    public float highVelocitySpinMultiplier = 1.5f;
    public Vector2 hitDampening = Vector2.one;
    public float hitMultiplier = 2;
    public float angularMultiplier = 5;

    public Rigidbody Body { get { return body; } }
    private Rigidbody body;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        body.velocity = initialVelocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        var newVelocity = body.velocity;
        var contact = collision.contacts[collision.contacts.Length - 1];
        var contactNormalDotForward = Vector3.Dot(contact.normal, Vector3.forward);
        var contactNormalDotUp = Vector3.Dot(contact.normal, Vector3.up);
        var contactNormalDotRight = Vector3.Dot(contact.normal, Vector3.right);

        // check for a hit at either end
        if (Mathf.Abs(contactNormalDotForward) > 0.9f)
        {
            var playerCharacter = collision.gameObject.GetComponent<PlayerInputMapper>();

            // ball collides with player
            // apply ball physics effects depending on player state
            if (playerCharacter)
            {
                var playerVelocity = playerCharacter.Velocity;

                if (playerCharacter.state.HasFlag(PlayerInputMapper.State.HIT))
                {
                    newVelocity.z *= hitMultiplier;
                    newVelocity.z = Mathf.Clamp(newVelocity.z, -hitMultiplier * initialVelocity.z, hitMultiplier * initialVelocity.z);

                    var spin = new Vector3(
                        playerVelocity.x * spinMultiplier * hitMultiplier * hitDampening.x,
                        playerVelocity.y * spinMultiplier * hitMultiplier * hitDampening.y,
                        0);
                    newVelocity -= spin;
                }
                else if (playerCharacter.state.HasFlag(PlayerInputMapper.State.DASH))
                {
                    newVelocity.x -= playerVelocity.x * spinMultiplier;
                    newVelocity.y -= playerVelocity.y * spinMultiplier;
                }
                else
                {
                    newVelocity.x -= playerVelocity.x * spinMultiplier;
                    newVelocity.y -= playerVelocity.y * spinMultiplier;

                    // cap/cushion velocity for collision with neutral player
                    if (newVelocity.z > 0)
                    {
                        newVelocity.z = Mathf.Max(initialVelocity.z, newVelocity.z);
                    }
                    else
                    {
                        newVelocity.z = Mathf.Min(-initialVelocity.z, newVelocity.z);
                    }
                }

                // apply spin effect via torque
                if (playerVelocity.sqrMagnitude > Mathf.Epsilon)
                {
                    var torque = new Vector3(playerVelocity.y, -playerVelocity.x, 0) * angularMultiplier;
                    StartCoroutine(Task.FixedUpdate(() => body.AddTorque(torque)));
                }
            }
        }

        // check for steep upward/downward deflection
        var steepDeflectValue = 0.65f;
        var velocityNerf = 0.6f;
        if (Mathf.Abs(contactNormalDotUp) > steepDeflectValue)
        {
            newVelocity.y *= velocityNerf;
            newVelocity.z = initialVelocity.z * Utils.SignMultiplier(newVelocity.z);
        }

        // check for steep leftward/rightward deflection
        if (Mathf.Abs(contactNormalDotRight) > steepDeflectValue)
        {
            newVelocity.x *= velocityNerf;
            newVelocity.z = initialVelocity.z * Utils.SignMultiplier(newVelocity.z);
        }

        body.velocity = newVelocity;
    }
}
