using UnityEngine;

public class BallLocal : MonoBehaviour
{
    [SerializeField] private Ball ball;
    [SerializeField] private Rigidbody body;

    private void Start()
    {
        ball.StartGame();

        // linear velocity
        StartCoroutine(Task.FixedUpdateContinuous(() =>
        {
            body.velocity = ball.Velocity;
        }));

        // rotation
        ball.OnAddForce += (force, mode) =>
        {
            body.AddForce(force, mode);
        };

        ball.OnAddTorque += (torque) =>
        {
            body.AddTorque(torque);
        };
    }
}
