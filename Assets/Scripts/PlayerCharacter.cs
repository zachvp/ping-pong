using UnityEngine;
using System;
using System.Collections;

// todo: refactor - too many lines
public class PlayerCharacter : MonoBehaviour
{
    public PlayerInputMapper input;

    public float moveSpeed = 5;

    public float dashMoveSpeedMultiplier = 1.5f;
    public int dashMoveFrames = 10;
    public int dashCooldownFrames = 18;

    public int hitDurationFrames = 24;
    public int hitStateBufferFrames = 12;

    public Vector3 Velocity { get; private set; }
    private Rigidbody body;

    public float stationaryFlickMultiplier = 2;

    public State state { get; private set; }
    public State cooldown;
    public State buffer;
    public IEnumerator bufferRoutine;

    public SharedVector3 dashDirection;

    public VisualsPlayerCharacter visuals;

    [Flags]
    public enum State
    {
        NONE = 0,
        HIT = 1 << 0,
        DASH = 1 << 1,
    }

    private void Awake()
    {
        body = GetComponentInParent<Rigidbody>();

        dashDirection.Reset();
    }

    private void Update()
    {
        if (!state.HasFlag(State.HIT) && input.isHitPressed)
        {
            state |= State.HIT;
            BufferState(state);

            visuals.Flash(hitDurationFrames);
            StartCoroutine(Task.Delayed(hitDurationFrames, () => state &= ~State.HIT));

        }

        var inputFlick = input.flick;
        if (!state.HasFlag(State.DASH) && !cooldown.HasFlag(State.DASH))
        {
            if (input.isFlick)
            {
                Dash(dashMoveFrames);
            }
        }
    }

    private void FixedUpdate()
    {
        var move = (Vector3)input.move;
        var velocity = move * moveSpeed;
        if (state.HasFlag(State.DASH))
        {
            velocity = input.FlickVelocityTriggered.normalized * moveSpeed * dashMoveSpeedMultiplier;
        }

        Velocity = velocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        var ball = collision.gameObject.GetComponent<Ball>();
        if (ball)
        {
            var zeroed = body.velocity;
            zeroed.z = 0;
            Velocity = zeroed;
        }
    }

    private void Dash(int dashFrameLength)
    {
        state |= State.DASH;
        cooldown |= State.DASH;
        BufferState(state);

        var roundedDashDirection = new Vector2(Mathf.Round(input.FlickVelocity.x),
                                               Mathf.Round(input.FlickVelocity.y));
        dashDirection.vector3.Set(roundedDashDirection);

        visuals.Trail(dashFrameLength);
        StartCoroutine(Task.Delayed(dashFrameLength, () =>
        {
            state &= ~State.DASH;
            input.ResetFlick();
        }));

        StartCoroutine(Task.Delayed(dashFrameLength + dashCooldownFrames, () =>
        {
            cooldown &= ~State.DASH;
            input.ResetFlick();
        }));
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
