using UnityEngine;

public class ScoreZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"trigger enter");
        var ball = other.GetComponent<Ball>();
        if (ball)
        {
            // todo:
            //HostGameState.Instance.AddScore();
            HostGameState.Instance.ResetGame();
        }
    }
}
