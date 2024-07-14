using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEngine.InputSystem.LowLevel;
using System.Collections;

// todo: refactor - too many lines
public class PlayerInputMapper : MonoBehaviour
{
    public float moveSpeed = 5;

    public float dashMoveSpeedMultiplier = 1.5f;
    public int dashMoveFrames = 10;
    public int dashCooldownFrames = 18;

    public int hitDurationFrames = 24;
    public int hitStateBufferFrames = 12;

    private PlayerInput input;

    public Vector3 Velocity { get; private set; }
    public Rigidbody Body { get { return body; } }
    private Rigidbody body;
    private Vector3 positionInitial;
    private Vector3 positionPrevious;

    private Material material;
    private Color initialColor;
    public string initialColorMaterialPropertyName = "_Color";
    public Color hitColor = Color.red;

    public Vector2 InputFlickVelocity { get; private set; }
    public Vector3 InputFlickVelocityDash { get; private set; }
    public float stationaryFlickMultiplier = 2;

    public State state;// { get; private set; }
    public State cooldowns;
    public State buffer;
    public IEnumerator bufferRoutine;

    private Vector3 cursorWorldPosition;
    private Vector3 cursorWorldVelocity;
    private Vector3 cursorWorldPositionPrevious;
    private Vector2 cursorPositionPrevious;
    private Vector2 cursorPositionSampled;
    public float cursorMoveThreshold = 0.1f;

    public TrailRenderer trailRenderer;

    public SharedVector3 dashDirection;

    // todo: dbg
    public DebugValues debugValues;

    [Flags]
    public enum State
    {
        NONE = 0,
        HIT = 1 << 0,
        DASH = 1 << 1,
    }

    private void Awake()
    {
        input = GetComponent<PlayerInput>();
        body = GetComponent<Rigidbody>();
        material = GetComponent<MeshRenderer>().material;

        initialColor = material.GetColor(initialColorMaterialPropertyName);
        positionInitial = body.position;

        dashDirection.Reset();
    }

    private void Update()
    {
        if (!state.HasFlag(State.HIT) && input.actions["hit"].WasPressedThisFrame())
        {
            state |= State.HIT;
            BufferState(state);

            hitColor.a = initialColor.a;
            material.SetColor(initialColorMaterialPropertyName, hitColor);
            //material.color = hitColor;
            StartCoroutine(Task.Delayed(hitDurationFrames, () =>
            {
                material.color = initialColor;
                state &= ~State.HIT;
            }));
        }

        var inputFlick = input.actions["flick"].ReadValue<Vector2>();
        InputFlickVelocity = inputFlick - InputFlickVelocity;
        if (!state.HasFlag(State.DASH) && !cooldowns.HasFlag(State.DASH))
        {
            if (InputFlickVelocity.sqrMagnitude > 0.25f)
            {
                InputFlickVelocityDash = InputFlickVelocity;
                Dash(dashMoveFrames);
            }
        }
        if (inputFlick.sqrMagnitude < Mathf.Epsilon)
        {
            StartCoroutine(Task.Delayed(0.2f, () =>
            {
                if (inputFlick.sqrMagnitude < Mathf.Epsilon)
                {
                    InputFlickVelocity = Vector2.zero;
                }
            }));
        }

        var cursor = input.actions["cursor-mouse"].ReadValue<Vector2>();
        var touch = input.actions["touch-cursor"].ReadValue<TouchState>();
        if (input.currentControlScheme.Equals("touchscreen"))
        {
            cursor = touch.position;
            Debug.Log($"touchscreen pos: {cursor}");
        }

        debugValues.vector2_0 = cursor;
        if (cursorPositionSampled.sqrMagnitude > cursorMoveThreshold)
        {
            var originalPosition = body.position;
            var newPosition = originalPosition;
            var zeroedPos = new Vector3(cursor.x, cursor.y, -input.camera.transform.position.z);
            newPosition = input.camera.ScreenToWorldPoint(zeroedPos);
            newPosition.z = positionInitial.z;
            cursorWorldPosition = newPosition;

            var deltaPosition = newPosition - originalPosition;
            if (deltaPosition.sqrMagnitude > 0.5f)
            {
                cursorWorldVelocity = (deltaPosition).normalized * moveSpeed;
            }
            else
            {
                cursorWorldVelocity = Vector3.zero;
            }

            debugValues.vector3 = newPosition;
        }

        if (!touch.isInProgress)
        {
            cursorWorldVelocity = Vector3.zero;
        }

        cursorPositionSampled = Common.FromFloat(cursor) - Common.FromFloat(cursorPositionPrevious);
        cursorPositionPrevious = cursor;

        debugValues.vector2_1 = cursorPositionSampled;
        debugValues.flt = cursorPositionSampled.sqrMagnitude;
    }

    private void FixedUpdate()
    {
        var move = (Vector3) input.actions["move"].ReadValue<Vector2>();
        debugValues.str = input.currentControlScheme;

        if (input.currentControlScheme.Equals("gamepad"))
        {
            if (state.HasFlag(State.DASH))
            {
                body.velocity = InputFlickVelocityDash.normalized * moveSpeed * dashMoveSpeedMultiplier;
            }
            else
            {
                body.velocity = move * moveSpeed;
            }
        }
        else
        {
            if (cursorPositionSampled.sqrMagnitude > cursorMoveThreshold)
                body.velocity = cursorWorldVelocity;
        }

        Velocity = body.velocity;
        positionPrevious = body.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        var ball = collision.gameObject.GetComponent<Ball>();
        if (ball)
        {
            body.velocity = Vector3.zero;
        }
    }

    private void Dash(int dashFrameLength)
    {
        state |= State.DASH;
        cooldowns |= State.DASH;
        BufferState(state);

        var roundedDashDirection = new Vector2(Mathf.Round(InputFlickVelocity.x), Mathf.Round(InputFlickVelocity.y));
        dashDirection.vector3.Set(roundedDashDirection);

        trailRenderer.enabled = true;
        trailRenderer.emitting = true;

        StartCoroutine(Task.Delayed(dashFrameLength * Constants.FRAME_TIME, () =>
        {
            trailRenderer.emitting = false;
            trailRenderer.enabled = false;

            InputFlickVelocity = Vector3.zero;
            state &= ~State.DASH;
        }));

        StartCoroutine(Task.Delayed(dashFrameLength + dashCooldownFrames, () =>
        {
            InputFlickVelocity = Vector3.zero;
            cooldowns &= ~State.DASH;
        }));
    }

    // todo: use and test this
    private State UpdateState(State current, State updated)
    {
        var resolved = current | updated;
        cooldowns |= updated;
        BufferState(resolved);

        return resolved;
    }

    private void BufferState(State s)
    {
        buffer |= s;
        
        Common.StopNullableCoroutine(this, bufferRoutine);
        bufferRoutine = Task.Delayed(hitStateBufferFrames, () =>
        {
            buffer &= ~s;
        });
        StartCoroutine(bufferRoutine);
    }
}
