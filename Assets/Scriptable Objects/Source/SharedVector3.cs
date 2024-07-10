using UnityEngine;

[CreateAssetMenu(
    fileName = nameof(SharedVector3),
    menuName = "ScriptableObject/" + nameof(SharedVector3))]
public class SharedVector3 : DataAsset
{
    public VarWatch<Vector3> vector3;

    public override void Reset()
    {
        vector3.Reset();
    }
}
