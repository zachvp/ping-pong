using Unity.Netcode;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static UnityEngine.UI.GridLayoutGroup;

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
            //var owner = Instantiate(ownerPrefab, ownerRoot.transform);
            
            ball.OnAddForce += (force, mode) =>
            {
                Debug.Log($"network add force {force}");
                body.AddForce(force, mode);
            };

            ball.OnAddTorque += (torque) =>
            {
                Debug.Log($"network add torque {torque}");
                body.AddTorque(torque);
            };
        }
        //UIDebug.Instance.Register($"Ball ClientID {OwnerClientId} Spawned", $"ObjectId: {NetworkObjectId}, Host: {IsHost}, Owner: {IsOwner}, Client: {IsClient}");
    }

    private void FixedUpdate()
    {
        if (IsOwner)
            body.velocity = ball.Velocity;
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void HandleGameResetClientRpc()
    {
        if (IsOwner)
        {
            Debug.Log($"ball handle reset");
            ball.Init();
            StartCoroutine(Task.FixedUpdate(() =>
            {
                body.velocity = Vector3.zero;
                body.position = ball.InitialPosition;
                body.rotation = Quaternion.identity;
                ball.Init();
            }));
        }
    }
}
