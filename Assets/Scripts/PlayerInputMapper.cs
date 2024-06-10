using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerInputMapper : MonoBehaviour
{
    public float moveSpeed = 5;

    public float dashMoveSpeedMultiplier = 1.5f;
    public float dashTimeLength = 0.5f;

    public float hitTimeLength = 0.4f;

    private PlayerInput input;

    public Vector3 Velocity { get; private set; }
    public Rigidbody Body { get { return body; } }
    private Rigidbody body;

    private Material material;
    private Color initialColor;
    public Color hitColor = Color.red;

    public Vector2 InputFlickVelocity { get; private set; }
    public Vector3 InputFlickVelocityDash { get; private set; }
    public float stationaryFlickMultiplier = 2;

    public State state;// { get; private set; }

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

        initialColor = material.color;
    }

    private void Update()
    {
        if (!state.HasFlag(State.HIT) && input.actions["hit"].WasPressedThisFrame())
        {
            state |= State.HIT;

            hitColor.a = initialColor.a;
            material.color = hitColor;
            StartCoroutine(Task.Delayed(hitTimeLength, () =>
            {
                material.color = initialColor;
                state &= ~State.HIT;
            }));
        }

        var inputFlick = input.actions["flick"].ReadValue<Vector2>();
        InputFlickVelocity = inputFlick - InputFlickVelocity;
        if (!state.HasFlag(State.DASH))
        {
            if (InputFlickVelocity.sqrMagnitude > 0.25f)
            {
                Dash();
                InputFlickVelocityDash = InputFlickVelocity;
            }
        }
    }

    private void Dash()
    {
        Debug.Log("DASH");
        state |= State.DASH;

        var newColor = Color.blue;
        newColor.a = initialColor.a;
        material.color = newColor;

        StartCoroutine(Task.Delayed(dashTimeLength, () =>
        {
            material.color = initialColor;
            state &= ~State.DASH;
            InputFlickVelocity = Vector3.zero;
        }));
    }

    private void FixedUpdate()
    {
        var move = (Vector3) input.actions["move"].ReadValue<Vector2>();

        if (state.HasFlag(State.DASH))
        {
            body.velocity = InputFlickVelocityDash * moveSpeed * dashMoveSpeedMultiplier;
        }
        else
        {
            body.velocity = move * moveSpeed;
        }

        Velocity = body.velocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        var ball = collision.gameObject.GetComponent<Ball>();
        if (ball)
        {
            body.velocity = Vector3.zero;
        }
    }
}
