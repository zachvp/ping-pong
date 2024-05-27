using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerInputMapper : MonoBehaviour
{
    public float moveSpeed = 5;

    public float dashSpeed = 8;
    public float dashTimeLength = 0.5f;

    public float hitTimeLength = 0.4f;

    private PlayerInput input;

    public Vector3 FrameVelocity { get; private set; }
    public Rigidbody Body { get { return body; } }
    private Rigidbody body;

    private Material material;
    private Color initialColor;

    public State state { get; private set; }

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

            var newColor = Color.yellow;
            newColor.a = initialColor.a;
            material.color = newColor;
            StartCoroutine(Task.Delayed(hitTimeLength, () =>
            {
                material.color = initialColor;
                state &= ~State.HIT;
            }));
        }

        if (!state.HasFlag(State.DASH) && input.actions["dash"].WasPressedThisFrame())
        {
            state |= State.DASH;

            var newColor = Color.blue;
            newColor.a = initialColor.a;
            material.color = newColor;
            StartCoroutine(Task.Delayed(dashTimeLength, () =>
            {
                material.color = initialColor;
                state &= ~State.DASH;
            }));
        }
    }

    private void FixedUpdate()
    {
        var move = (Vector3) input.actions["move"].ReadValue<Vector2>();

        if (move.sqrMagnitude > Mathf.Epsilon) 
        {
            var lastPosition = body.position;
            Vector3 newPosition;

            if (state == State.DASH)
            {
                newPosition = body.position + move * Time.deltaTime * dashSpeed;
            }
            else
            {
                newPosition = body.position + move * Time.deltaTime * moveSpeed;
            }

            body.MovePosition(newPosition);
            FrameVelocity = newPosition - lastPosition;
        }
        else
        {
            body.velocity = Vector3.zero;
        }
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
