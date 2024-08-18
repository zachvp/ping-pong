using UnityEngine;

public class VisualsPlayerCharacter : MonoBehaviour
{
    private Material material;
    private Color initialColor;
    public string initialColorMaterialPropertyName = "_Color";
    public Color hitColor = Color.red;
    public TrailRenderer trailRenderer;

    private void Awake()
    {
        material = GetComponentInParent<MeshRenderer>().material;

        initialColor = material.GetColor(initialColorMaterialPropertyName);
    }

    public void Flash(int durationFrames)
    {
        hitColor.a = initialColor.a;
        material.SetColor(initialColorMaterialPropertyName, hitColor);
        StartCoroutine(Task.Delayed(durationFrames, () => material.color = initialColor));
    }

    public void Trail(int durationFrames)
    {
        trailRenderer.enabled = true;
        trailRenderer.emitting = true;

        StartCoroutine(Task.Delayed(durationFrames, () =>
        {
            trailRenderer.emitting = false;
            trailRenderer.enabled = false;
        }));
    }
}
