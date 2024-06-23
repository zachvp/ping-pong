using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class RandomBS : MonoBehaviour
{
    void Update()
    {
        if (Keyboard.current.backspaceKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
}
