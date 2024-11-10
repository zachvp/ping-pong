using UnityEngine;

public class PlayerCharacterLocal : MonoBehaviour
{
    [SerializeField] private PlayerCharacter character;
    [SerializeField] private Rigidbody body;

    private void Start()
    {
        StartCoroutine(Task.FixedUpdateContinuous(() =>
        {
            body.velocity = character.Velocity;
        }));
    }
}
