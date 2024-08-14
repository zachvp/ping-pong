using UnityEngine;

public class DisableVisuals : MonoBehaviour
{
    private void Awake()
    {
        var renderer = GetComponent<Renderer>();
        renderer.enabled = false;
    }
}
