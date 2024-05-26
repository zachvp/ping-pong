using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputMapper : MonoBehaviour
{
    public float speed = 2;

    private PlayerInput input;
    private Rigidbody body;

    void Awake()
    {
        input = GetComponent<PlayerInput>();
        body = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        var move = (Vector3) input.actions["move"].ReadValue<Vector2>();

        body.MovePosition(body.position + move * Time.deltaTime * speed);
    }
}
