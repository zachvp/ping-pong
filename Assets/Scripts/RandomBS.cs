using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class RandomBS : MonoBehaviour
{
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

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 700, 150, 200));
        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fontSize = 32;
        if (GUILayout.Button("Restart", buttonStyle, GUILayout.Width(150), GUILayout.Height(200)))
        {
            Restart();
        }
        GUILayout.EndArea();
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
