using UnityEngine;

[CreateAssetMenu(
    fileName = nameof(PlayerSharedState),
    menuName = "ScriptableObject/" + nameof(PlayerSharedState))]
public class PlayerSharedState : ScriptableObject
{
    public int cameraForwardZ;
    public Vector3 velocity;
    public PlayerCharacter.State state;
    public PlayerCharacter.State buffer;
}
