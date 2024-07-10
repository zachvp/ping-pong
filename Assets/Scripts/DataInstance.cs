using UnityEngine;

public abstract class DataInstance : MonoBehaviour
{
    public string label = "DEFAULT";

    public abstract float Normalized { get; }
}
