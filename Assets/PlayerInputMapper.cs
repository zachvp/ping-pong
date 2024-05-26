using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputMapper : MonoBehaviour
{
    public float speed = 2;

    private PlayerInput input;

    public Vector3 FrameVelocity { get; private set; }
    public Rigidbody Body { get { return body; } }
    private Rigidbody body;

    void Awake()
    {
        input = GetComponent<PlayerInput>();
        body = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        var move = (Vector3) input.actions["move"].ReadValue<Vector2>();

        var lastPosition = body.position;
        var newPosition = body.position + move * Time.deltaTime * speed;
        body.MovePosition(newPosition);
        FrameVelocity = newPosition - lastPosition;

        //Debug.Log($"player velocity: {body.velocity}");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Ball>()) {
            body.velocity = Vector3.zero;
        }
    }
}
