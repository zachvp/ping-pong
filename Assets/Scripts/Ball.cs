using System.Collections;
using UnityEngine;
using System;

public class Ball : MonoBehaviour
{
    public Vector3 defaultSpinSpeed = new Vector2(10, 10);
    public Vector3 bigSpinSpeed = new Vector2(15, 15);

    public Vector3 defaultCurveSpeed = new Vector2(10, 10);
    public Vector3 bigCurveSpeed = new Vector2(15, 15);

    public int curveLateBufferFrames = 12;

    [Tooltip("Determines how much spin torque to apply on player hit.")]
    public float curveSpinAngularMultiplier = 10;
    [Tooltip("Determines the force the ball will drive to the center of its curve arc.")]
    public float curveVelocityMultiplier = 1;
    [Tooltip("The time length of the curve.")]
    public int curveDurationFrames = 120;
    
    public float hitMultiplier = 2;
    public int hitStackCount = 4;
    public Vector2 hitDampening = Vector2.one;
    public int hitLateBufferFrames = 10;
    public int hitCooldownFrames = 15;

    public Vector3 initialVelocity;
    public Vector3 InitialPosition { get; private set; }
    public Vector3 minSpeeds;

    private Rigidbody body;

    private IEnumerator currentCurveCoroutine;

    public State state;
    public State cooldown;
    private Vector3 playerVelocity;

    public float steepDeflectDotValue = 0.95f;
    public float steepDeflectVelocityNerf = 0.65f;

    public Vector3 Velocity { get; private set; }
    public Action<Vector3, ForceMode> OnAddForce;
    public Action<Vector3> OnAddTorque;

    // TODO: DBG
    public float debugDuration = 2;
    public DebugValues debugValues;

    private void Awake()
    {
        body = GetComponentInParent<Rigidbody>();
        InitialPosition = transform.position;
    }

    private void Start()
    {
        Reset();
    }

    public void Reset()
    {
        state |= State.STOPPED;
        Velocity = Vector3.zero;
        Common.StopNullableCoroutine(this, currentCurveCoroutine);
    }

    public void StartGame()
    {
        state &= ~State.STOPPED;
        Velocity = initialVelocity;
    }

    private void Update()
    {
        debugValues.str_0 = state.ToString();

        if (state.HasFlag(State.HIT) && !cooldown.HasFlag(State.HIT))
        {
            //Debug.Log($"handle hit for state: {state}");
            state &= ~State.HIT;
            cooldown |= State.HIT;

            var newVelocity = body.velocity;
            var maxSpeedZ = hitMultiplier * Mathf.Abs(initialVelocity.z) * hitStackCount;
            var resolvedCurveSpeed = Common.SignMultiply(defaultCurveSpeed, playerVelocity);

            newVelocity.z *= hitMultiplier;
            newVelocity.z = Mathf.Clamp(newVelocity.z, -maxSpeedZ, maxSpeedZ);

            newVelocity += Vector3.Scale(Common.Round(playerVelocity.normalized), defaultSpinSpeed);
            newVelocity.x = Mathf.Clamp(newVelocity.x, -defaultSpinSpeed.x * 1.5f, defaultSpinSpeed.x * 1.5f);
            newVelocity.y = Mathf.Clamp(newVelocity.y, -defaultSpinSpeed.y * 1.5f, defaultSpinSpeed.y * 1.5f);

            ApplySpinCurve(newVelocity, resolvedCurveSpeed, 2);
            StartCoroutine(Task.FixedUpdate(() => Velocity = newVelocity));
            StartCoroutine(Task.Delayed(hitCooldownFrames, () => cooldown &= ~State.HIT));
        }

        if (state.HasFlag(State.SPIN) && !cooldown.HasFlag(State.SPIN))
        {
            state &= ~State.SPIN;
            cooldown |= State.SPIN;

            var newVelocity = body.velocity + Vector3.Scale(Common.Round(playerVelocity.normalized), bigSpinSpeed);
            newVelocity.x = Mathf.Clamp(newVelocity.x, -bigSpinSpeed.x * 1.5f, bigSpinSpeed.x * 1.5f);
            newVelocity.y = Mathf.Clamp(newVelocity.y, -bigSpinSpeed.y * 1.5f, bigSpinSpeed.y * 1.5f);
            newVelocity.z = body.velocity.z;

            var resolvedCurveSpeed = Common.SignMultiply(bigCurveSpeed, playerVelocity);
            resolvedCurveSpeed.z = 0;

            ApplySpinCurve(newVelocity, resolvedCurveSpeed, 2);
            StartCoroutine(Task.FixedUpdate(() => Velocity = newVelocity));
            StartCoroutine(Task.Delayed(hitCooldownFrames, () => cooldown &= ~State.SPIN));
        }
    }

    private void FixedUpdate()
    {
        Debug.DrawLine(body.position, body.position + Vector3.forward * 0.15f, Color.blue, debugDuration);

        debugValues.flt = Mathf.Max(debugValues.flt, body.velocity.z);

        // enforce min speed
        if (!state.HasFlag(State.STOPPED) && Mathf.Abs(body.velocity.z) < minSpeeds.z)
        {
            var newVelocity = body.velocity;
            newVelocity.z = Common.SignMultiply(body.velocity.z) * minSpeeds.z;
            Velocity = newVelocity;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        var newVelocity = body.velocity;
        var contact = collision.contacts[^1];
        var contactNormalDotForward = Vector3.Dot(contact.normal, Vector3.forward);

        // check for a hit at either end
        if (Mathf.Abs(contactNormalDotForward) > 0.9f)
        {
            Common.StopNullableCoroutine(this, currentCurveCoroutine);

            var networkState = collision.gameObject.GetComponentInChildren<NetworkPlayerSharedState>();

            // ball collides with player
            // apply ball curve physics effects depending on player state
            if (networkState)
            {
                playerVelocity = networkState.velocity.Value;
                Debug.Log($"playerVelocity: {playerVelocity}; bodyVelocity: {body.velocity}");
                playerVelocity = Common.SignMultiply(playerVelocity, networkState.cameraForwardZ.Value);
                var playerState = networkState.state.Value;
                var playerStateBuffer = networkState.buffer.Value;

                if (playerState.HasFlag(PlayerCharacter.State.HIT) &&
                    !(state | cooldown).HasFlag(State.HIT))
                {
                    state |= State.HIT;
                }
                else if (playerState.HasFlag(PlayerCharacter.State.DASH) &&
                    !(state | cooldown).HasFlag(State.SPIN))
                {
                    state |= State.SPIN;
                }
                else
                {
                    var resolvedCurveSpeed = Common.SignMultiply(defaultCurveSpeed, playerVelocity);
                    newVelocity += Vector3.Scale(Common.Round(playerVelocity.normalized), defaultSpinSpeed);
                    newVelocity.x = Mathf.Clamp(newVelocity.x, -defaultSpinSpeed.x * 1.5f, defaultSpinSpeed.x * 1.5f);
                    newVelocity.y = Mathf.Clamp(newVelocity.y, -defaultSpinSpeed.y * 1.5f, defaultSpinSpeed.y * 1.5f);

                    Debug.Log($"apply normal hit spin; vel: {newVelocity}, curveSpeed: {resolvedCurveSpeed}");
                    ApplySpinCurve(newVelocity, resolvedCurveSpeed, 2);
                }

                // Player state buffer checks
                // check for a hit in the player state buffer to forgive slightly late timing
                StartCoroutine(Task.Continuous(hitLateBufferFrames, () =>
                {
                    if (playerStateBuffer.HasFlag(PlayerCharacter.State.HIT) &&
                        !(state | cooldown).HasFlag(State.HIT))
                    {
                        //Debug.Log($"set hit input for state: {state}");
                        state |= State.HIT;
                    }
                }));

                StartCoroutine(Task.Continuous(curveLateBufferFrames, () =>
                {
                    if (playerStateBuffer.HasFlag(PlayerCharacter.State.DASH) &&
                        !(state | cooldown).HasFlag(State.SPIN))
                    {
                        //Debug.Log($"set hit input for state: {state}");
                        state |= State.SPIN;
                    }
                }));
            }
        }
        else
        {
            Common.StopNullableCoroutine(this, currentCurveCoroutine);

            // check for steep upward/downward deflection
            var contactNormalDotUp = Vector3.Dot(contact.normal, Vector3.up);
            var contactNormalDotRight = Vector3.Dot(contact.normal, Vector3.right);
            if (Mathf.Abs(contactNormalDotUp) > steepDeflectDotValue)
            {
                newVelocity.y *= steepDeflectVelocityNerf;
                //Debug.LogFormat($"steep deflection: {Mathf.Abs(contactNormalDotUp)}");
            }

            // check for steep leftward/rightward deflection
            if (Mathf.Abs(contactNormalDotRight) > steepDeflectDotValue)
            {
                newVelocity.x *= steepDeflectVelocityNerf;
                //Debug.LogFormat($"steep deflection: {Mathf.Abs(contactNormalDotRight)}");
            }
        }

        Velocity = newVelocity;
    }

    private void ApplySpinCurve(Vector3 spinVelocity, Vector3 curveVelocity, int curveTickSteps)
    {
        // apply spin + curve effect
        if (playerVelocity.sqrMagnitude > Mathf.Epsilon)
        {
            var torque = new Vector3(spinVelocity.y, -spinVelocity.x, 0) * curveSpinAngularMultiplier;

            // apply torque so the ball spins
            //Debug.Log($"addTorque: {torque}");
            StartCoroutine(Task.FixedUpdate(() => OnAddTorque?.Invoke(torque)));

            // apply centripetal force over time for a curved motion
            Common.StopNullableCoroutine(this, currentCurveCoroutine);
            currentCurveCoroutine = CurveCoroutine(curveVelocity, curveTickSteps, curveDurationFrames);
            StartCoroutine(currentCurveCoroutine);
        }
    }

    private IEnumerator CurveCoroutine(Vector3 curveVelocity, int tickStep, int durationFrames)
    {
        // at any given frame, the force is directed to the "center" relative to the ball's velocity
        var curveMotion = -curveVelocity * curveVelocityMultiplier;
        curveMotion.z = 0;

        var tick = 0;
        return Task.FixedUpdate(durationFrames, () =>
        {
            tick++;
            if (tick % tickStep == 0)
            {
                //Debug.Log($"addForce: {curveMotion}");
                OnAddForce?.Invoke(curveMotion, ForceMode.Acceleration);
            }
        });
    }

    [Flags]
    public enum State
    {
        NONE = 0,
        HIT = 1 << 1,
        SPIN = 1 << 2,
        STOPPED = 1 << 3,
    }
}
