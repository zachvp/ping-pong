using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputMapper : MonoBehaviour
{
    private PlayerInput input;

    public bool isHitPressed;
    //public Action OnHitPressed; // todo: convert to event listener from PlayerInput

    // todo: remove 'input' from name prefixes
    public Vector2 inputFlick;
    public Vector2 InputFlickVelocity { get; private set; }
    public float inputFlickVelocityThreshold = 0.25f;
    public Vector3 InputFlickVelocityDash { get; private set; } // todo: replace 'dash' with 'flick'

    public bool isFlick;

    public Vector2 move;

    // todo: dbg
    public DebugValues debugValues;

    private void Awake()
    {
        input = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        debugValues.str_0 = input.currentControlScheme;
        debugValues.str_1 = input.currentActionMap.name;
        debugValues.vector2_0 = InputFlickVelocity;

        isHitPressed = input.actions["hit"].WasPressedThisFrame();

        inputFlick = input.actions["flick"].ReadValue<Vector2>();
        InputFlickVelocity = inputFlick - InputFlickVelocity;

        isFlick = InputFlickVelocity.sqrMagnitude > inputFlickVelocityThreshold;
        if (isFlick)
        {
            InputFlickVelocityDash = InputFlickVelocity;
        }

        move = input.actions["move"].ReadValue<Vector2>();

        // touchscreen
        if (input.currentControlScheme.Equals("touchscreen"))
        {
            var touch = input.actions["touch.tap"];
            isHitPressed = touch.WasPressedThisFrame();
            if (isHitPressed)
            {
                Debug.Log("tap detected");
            }
        }
    }

    public void ResetFlick()
    {
        InputFlickVelocity = Vector3.zero;
    }
}
