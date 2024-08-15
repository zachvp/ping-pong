using UnityEngine;

public class ScoreZone : MonoBehaviour
{
    public int playerID;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"trigger enter");
        var ball = other.GetComponent<Ball>();
        if (ball)
        {
            // todo: implement score for real
            var hostGameState = HostGameState.Instance;
            hostGameState.AddScoreRpc(playerID, 1);
            StartCoroutine(Task.Delayed(hostGameState.restartDelay * 2, () => hostGameState.StartGame()));
        }
    }
}
