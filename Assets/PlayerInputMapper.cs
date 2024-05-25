using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputMapper : MonoBehaviour
{
    private PlayerInput input;

    void Awake()
    {
        input = GetComponent<PlayerInput>();
    }

    void Update()
    {
        Debug.Log(input.actions["move"].ReadValue<Vector2>());

        //if ()
        {

        }
    }
}
