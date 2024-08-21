using UnityEngine;

public class VisualsPlayerCharacter : MonoBehaviour
{
    private Material material;
    private Color initialColor;
    public string initialColorMaterialPropertyName = "_Color";
    public Color hitColor = Color.red;
    public TrailRenderer trailRenderer0;
    public TrailRenderer trailRenderer1;
    public OffsetTransform offset0;
    public OffsetTransform offset1;

    private void Awake()
    {
        material = GetComponentInParent<MeshRenderer>().material;

        initialColor = material.GetColor(initialColorMaterialPropertyName);
        offset0.SetOffset(Vector3.left);
        offset1.SetOffset(Vector3.left);
    }

    public void Flash(int durationFrames)
    {
        hitColor.a = initialColor.a;
        material.SetColor(initialColorMaterialPropertyName, hitColor);
        StartCoroutine(Task.Delayed(durationFrames, () => material.color = initialColor));
    }

    public void Trail(int durationFrames, Vector3 direction)
    {
        offset0.SetOffset(direction);
        offset1.SetOffset(direction);
        trailRenderer0.enabled = true;
        trailRenderer1.enabled = true;

        trailRenderer0.emitting = true;
        trailRenderer1.emitting = true;

        StartCoroutine(Task.Delayed(durationFrames, () =>
        {
            trailRenderer0.emitting = false;
            trailRenderer1.emitting = false;

            trailRenderer0.enabled = false;
            trailRenderer1.enabled = false;
        }));
    }
}
