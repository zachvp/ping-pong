using UnityEngine;
using System;
using UnityEngine.InputSystem.LowLevel;
using System.Collections;

// todo: refactor - too many lines
public class PlayerCharacter : MonoBehaviour
{
    public PlayerInputMapper inputMapper; // todo: rename to 'input'

    public float moveSpeed = 5;

    public float dashMoveSpeedMultiplier = 1.5f;
    public int dashMoveFrames = 10;
    public int dashCooldownFrames = 18;

    public int hitDurationFrames = 24;
    public int hitStateBufferFrames = 12;

    public Vector3 Velocity { get; private set; }
    public Rigidbody Body { get { return body; } }
    private Rigidbody body;
    private Vector3 positionInitial;
    private Vector3 positionPrevious;

    private Material material;
    private Color initialColor;
    public string initialColorMaterialPropertyName = "_Color";
    public Color hitColor = Color.red;

    public float stationaryFlickMultiplier = 2;

    public State state { get; private set; }
    public State cooldown;
    public State buffer;
    public IEnumerator bufferRoutine;

    public TrailRenderer trailRenderer;

    public SharedVector3 dashDirection;

    [Flags]
    public enum State
    {
        NONE = 0,
        HIT = 1 << 0,
        DASH = 1 << 1,
    }

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        material = GetComponent<MeshRenderer>().material;

        initialColor = material.GetColor(initialColorMaterialPropertyName);
        positionInitial = body.position;

        dashDirection.Reset();
    }

    private void Update()
    {

        if (!state.HasFlag(State.HIT) && inputMapper.isHitPressed)
        {
            state |= State.HIT;
            BufferState(state);

            hitColor.a = initialColor.a;
            material.SetColor(initialColorMaterialPropertyName, hitColor);
            StartCoroutine(Task.Delayed(hitDurationFrames, () =>
            {
                material.color = initialColor;
                state &= ~State.HIT;
            }));
        }

        var inputFlick = inputMapper.inputFlick;
        if (!state.HasFlag(State.DASH) && !cooldown.HasFlag(State.DASH))
        {
            if (inputMapper.isFlick)
            {
                Dash(dashMoveFrames);
            }
        }
    }

    private void FixedUpdate()
    {
        var move = (Vector3) inputMapper.move;
        if (state.HasFlag(State.DASH))
        {
            body.velocity = inputMapper.InputFlickVelocityDash.normalized * moveSpeed * dashMoveSpeedMultiplier;
        }
        else
        {
            body.velocity = move * moveSpeed;
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
        cooldown |= State.DASH;
        BufferState(state);

        var roundedDashDirection = new Vector2(Mathf.Round(inputMapper.InputFlickVelocity.x),
                                               Mathf.Round(inputMapper.InputFlickVelocity.y));
        dashDirection.vector3.Set(roundedDashDirection);

        trailRenderer.enabled = true;
        trailRenderer.emitting = true;

        StartCoroutine(Task.Delayed(dashFrameLength, () =>
        {
            trailRenderer.emitting = false;
            trailRenderer.enabled = false;

            state &= ~State.DASH;
            inputMapper.ResetFlick();
        }));

        StartCoroutine(Task.Delayed(dashFrameLength + dashCooldownFrames, () =>
        {
            cooldown &= ~State.DASH;
            inputMapper.ResetFlick();
        }));
    }

    // todo: use and test this
    private State UpdateState(State current, State updated)
    {
        var resolved = current | updated;
        cooldown |= updated;
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
