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
    }
}
