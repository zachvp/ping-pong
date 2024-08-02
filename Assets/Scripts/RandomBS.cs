using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RandomBS : MonoBehaviour
{
    public Button restart;

    private void Awake()
    {
        restart.onClick.AddListener(() =>
        {
            Restart();
        });
    }

    void Update()
    {
        if (Keyboard.current.backspaceKey.wasPressedThisFrame)
        {
            Restart();
        }
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (Time.timeScale > 0)
            {
                Time.timeScale = 0;

            }
            else
            {
                Time.timeScale = 1;
            }
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
