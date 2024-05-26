using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputMapper : MonoBehaviour
{
    public float speed = 2;

    private PlayerInput input;

    public Vector3 FrameVelocity { get; private set; }
    public Rigidbody Body { get { return body; } }
    private Rigidbody body;

    private Material material;

    public State state { get; private set; }
    public enum State
    {
        NONE,
        HIT
    }

    private void Awake()
    {
        input = GetComponent<PlayerInput>();
        body = GetComponent<Rigidbody>();
        material = GetComponent<MeshRenderer>().material;
    }

    private void Update()
    {
        if (input.actions["hit"].WasPressedThisFrame())
        {
            var initialColor = material.color;
            var newColor = Color.yellow;
            newColor.a = initialColor.a;

            material.color = newColor;
            state = State.HIT;
            StartCoroutine(Task.Delayed(0.8f, () =>
            {
                material.color = initialColor;
                state = State.NONE;
            }));
        }
    }

    private void FixedUpdate()
    {
        var move = (Vector3) input.actions["move"].ReadValue<Vector2>();

        if (move.sqrMagnitude > Mathf.Epsilon) 
        {
            var lastPosition = body.position;
            var newPosition = body.position + move * Time.deltaTime * speed;
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
