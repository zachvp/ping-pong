using TMPro;
using UnityEngine;

public class UIScore : MonoBehaviour
{
    public TextMeshProUGUI score;
    public int playerID;

    private void Start()
    {
        score.text = $"{0}";

        switch (playerID)
        {
            case 0:
                HostGameState.Instance.scorePlayer0.OnValueChanged += (old, current) =>
                {
                    score.text = $"{current}";
                };
                break;
            case 1:
                HostGameState.Instance.scorePlayer1.OnValueChanged += (old, current) =>
                {
                    score.text = $"{current}";
                };
                break;
        }
    }
}
