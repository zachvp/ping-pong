using Unity.Netcode;
using UnityEngine;

// todo: rename to NetworkScoreZone
public class ScoreZone : NetworkBehaviour
{
    public int playerID;

    private void OnTriggerEnter(Collider other)
    {
        var ball = other.GetComponent<NetworkBall>();
        var hostGameState = HostGameState.Instance;

        if (ball && IsOwner && hostGameState.state == GameState.MAIN)
        {
            Debug.Log($"client {OwnerClientId} add score for player: {playerID}");
            // todo: implement score for real
            hostGameState.AddScoreRpc(playerID, 1);
            StartCoroutine(Task.Delayed(hostGameState.restartDelay * 2, () => hostGameState.StartGame()));
        }
    }
}
