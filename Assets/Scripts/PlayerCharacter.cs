using UnityEngine;
using System;
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
    private Rigidbody body;

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

    private NetworkPlayer network;

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
        network = GetComponent<NetworkPlayer>();

        initialColor = material.GetColor(initialColorMaterialPropertyName);

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
        if (network.IsClient && network.IsOwner)
        {
            var move = (Vector3)inputMapper.move;
            var velocity = move * moveSpeed;
            if (state.HasFlag(State.DASH))
            {
                velocity = inputMapper.InputFlickVelocityTriggered.normalized * moveSpeed * dashMoveSpeedMultiplier;
            }

            Velocity = velocity;

            network.UpdateServerRPC(Velocity);
            //Debug.Log($"update server velocity: {Velocity} for input move {move}");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        var ball = collision.gameObject.GetComponent<Ball>();
        if (ball)
        {
            var zeroed = body.velocity;
            zeroed.z = 0;
            body.velocity = zeroed;
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
    private State AddState(State current, State added)
    {
        var resolved = current | added;
        cooldown |= added;
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
