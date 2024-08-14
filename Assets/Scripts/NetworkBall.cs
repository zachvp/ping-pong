using Unity.Netcode;
using UnityEngine;

public class NetworkBall : NetworkBehaviour, INetworkGameStateHandler
{
    public GameObject ownerPrefab;
    public GameObject ownerRoot;

    private Rigidbody body;
    private Ball ball;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        ball = GetComponent<Ball>();
    }

    private void Start()
    {
        HostGameState.Instance.RegisterGameResetHandler(this);
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
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

    private void FixedUpdate()
    {
        if (IsOwner)
            body.velocity = ball.Velocity;
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void HandleGameStateChangeRpc(GameState old, GameState current)
    {
        if (IsOwner)
        {
            switch (current)
            {
                case GameState.MAIN:
                    ball.StartGame();
                    break;
                case GameState.RESET:
                case GameState.SCORE:
                    ball.Reset();
                    StartCoroutine(Task.FixedUpdate(() =>
                    {
                        body.velocity = Vector3.zero;
                        body.position = ball.InitialPosition;
                        body.rotation = Quaternion.identity;
                        body.angularVelocity = Vector3.zero;
                    }));
                    break;
            }

            Debug.Log($"ball handle reset");
        }
    }
}
