using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [Tooltip("Determines player velocity effect on ball curve.")]
    public float curveMultiplier = 0.5f;
    public float dashSpinMultiplier = 8;

    [Tooltip("Determines how much spin torque to apply on player hit.")]
    public float curveSpinAngularMultiplier = 10;
    [Tooltip("Determines the force the ball will drive to the center of its curve arc.")]
    public float centripetalForceMultiplier = 2;
    
    public float hitMultiplier = 2;
    public Vector2 hitDampening = Vector2.one;
    public int hitLateBufferFrames = 10;

    public Vector3 initialVelocity;
    private Vector3 initialPosition;

    public Rigidbody Body { get { return body; } }
    private Rigidbody body;

    private IEnumerator currentCurveCoroutine;

    // TODO: DBG
    public float debugDuration = 2;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        initialPosition = transform.position;
    }

    private void Start()
    {
        body.velocity = initialVelocity;
    }

    private void FixedUpdate()
    {
        Debug.DrawLine(body.position, body.position + Vector3.forward * 0.15f, Color.blue, debugDuration);

        // cap/cushion velocity for collision with neutral player
        var newVelocity = body.velocity;
        if (newVelocity.z < initialVelocity.z * hitMultiplier)
        {
            if (newVelocity.z > 0)
            {
                newVelocity.z = Mathf.Max(initialVelocity.z, newVelocity.z);
            }
            else
            {
                newVelocity.z = Mathf.Min(-initialVelocity.z, newVelocity.z);
            }
            body.velocity = newVelocity;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        var newVelocity = body.velocity;
        var contact = collision.contacts[collision.contacts.Length - 1];
        var contactNormalDotForward = Vector3.Dot(contact.normal, Vector3.forward);

        // check for a hit at either end
        if (Mathf.Abs(contactNormalDotForward) > 0.9f)
        {
            StopNullableCoroutine(currentCurveCoroutine);

            var playerCharacter = collision.gameObject.GetComponent<PlayerInputMapper>();

            // ball collides with player
            // apply ball curve physics effects depending on player state
            if (playerCharacter)
            {
                var playerVelocity = playerCharacter.Velocity;

                if (playerCharacter.state.HasFlag(PlayerInputMapper.State.HIT))
                {
                    newVelocity.z *= hitMultiplier;
                    newVelocity.z = Mathf.Clamp(newVelocity.z, -hitMultiplier * initialVelocity.z, hitMultiplier * initialVelocity.z);

                    var spin = new Vector3(
                        playerVelocity.x * curveMultiplier * hitDampening.x,
                        playerVelocity.y * curveMultiplier * hitDampening.y,
                        0);
                    newVelocity -= spin;
                }
                else if (playerCharacter.state.HasFlag(PlayerInputMapper.State.DASH))
                {
                    newVelocity.x += playerVelocity.x * dashSpinMultiplier;
                    newVelocity.y -= playerVelocity.y * dashSpinMultiplier;
                }
                else
                {
                    newVelocity.x += playerVelocity.x * curveMultiplier;
                    newVelocity.y -= playerVelocity.y * curveMultiplier;
                }

                if (playerVelocity.sqrMagnitude > Mathf.Epsilon)
                {
                    // apply spin effect via torque
                    var torque = new Vector3(playerVelocity.y, -playerVelocity.x, 0) * curveSpinAngularMultiplier;
                    StartCoroutine(Task.FixedUpdate(() => body.AddTorque(torque)));

                    // apply centripetal force for a curved motion
                    currentCurveCoroutine = CurveCoroutine(newVelocity);
                    StartCoroutine(currentCurveCoroutine);
                }

                // check for a hit in the player state buffer to forgive slightly late timing
                StartCoroutine(Task.Continuous(hitLateBufferFrames, () =>
                {
                    var hitSpeed = Mathf.Abs(hitMultiplier * initialVelocity.z);
                    if (playerCharacter.buffer.HasFlag(PlayerInputMapper.State.HIT) &&
                        Mathf.Abs(body.velocity.z) < hitSpeed)
                    {
                        Debug.Log($"apply hit in collision buffer");
                        newVelocity.z *= hitMultiplier;
                        newVelocity.z = Mathf.Clamp(newVelocity.z, -hitSpeed, hitSpeed);

                        var spin = new Vector3(
                            playerVelocity.x * curveMultiplier * hitDampening.x,
                            playerVelocity.y * curveMultiplier * hitDampening.y,
                            0);
                        newVelocity -= spin;

                        body.velocity = newVelocity;
                    }
                }));
            }
        }
        else
        {
            StopNullableCoroutine(currentCurveCoroutine);

            // check for steep upward/downward deflection
            var contactNormalDotUp = Vector3.Dot(contact.normal, Vector3.up);
            var contactNormalDotRight = Vector3.Dot(contact.normal, Vector3.right);
            var steepDeflectValue = 0.95f;
            var velocityNerf = 0.6f;
            if (Mathf.Abs(contactNormalDotUp) > steepDeflectValue)
            {
                newVelocity.y *= velocityNerf;
                newVelocity.z = initialVelocity.z * Utils.SignMultiplier(newVelocity.z);

                //Debug.LogFormat($"steep deflection: {Mathf.Abs(contactNormalDotUp)}");
            }

            // check for steep leftward/rightward deflection
            if (Mathf.Abs(contactNormalDotRight) > steepDeflectValue)
            {
                newVelocity.x *= velocityNerf;
                newVelocity.z = initialVelocity.z * Utils.SignMultiplier(newVelocity.z);

                //Debug.LogFormat($"steep deflection: {Mathf.Abs(contactNormalDotRight)}");
            }
        }

        body.velocity = newVelocity;
    }

    // todo: move to shared class
    private void StopNullableCoroutine(IEnumerator enumerator)
    {
        if (enumerator != null)
        {
            StopCoroutine(enumerator);
        }
    }

    private IEnumerator CurveCoroutine(Vector3 spinVelocity)
    {
        // at any given frame, the force is directed to the "center" relative to the ball's velocity
        var centripetalForce = -spinVelocity;
        centripetalForce.z = 0;

        var tick = 0;
        var tickStep = 2;
        return Task.FixedUpdateContinuous(() =>
        {
            tick++;
            if (tick % tickStep == 0)
            {
                body.AddForce(centripetalForce * centripetalForceMultiplier, ForceMode.Acceleration);
            }
        });
    }
}
