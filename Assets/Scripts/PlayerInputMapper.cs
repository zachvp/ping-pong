using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

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

        isHitPressed = input.actions["hit"].WasPressedThisFrame();

        inputFlick = input.actions["flick"].ReadValue<Vector2>();
        InputFlickVelocity = inputFlick - InputFlickVelocity;

        isFlick = InputFlickVelocity.sqrMagnitude > inputFlickVelocityThreshold;
        if (isFlick)
        {
            InputFlickVelocityDash = InputFlickVelocity;
        }

        move = input.actions["move"].ReadValue<Vector2>();  // todo: rename to cursor0, cursor1

        // touchscreen
        if (input.currentControlScheme.Equals("touchscreen"))
        {
            var tap = input.actions["touch.tap"];
            isHitPressed = tap.WasPressedThisFrame();
            if (isHitPressed)
            {
                Debug.Log("tap detected");
            }

            var touch = input.actions["touch.move"].ReadValue<TouchState>();
            debugValues.bool_0 = touch.isInProgress;

            if (touch.isInProgress)
            {
                // check for a move cursor...
                if (touch.position.x < Screen.width / 2)
                {
                    if (touchMoveOrigin.sqrMagnitude > Mathf.Epsilon)
                    {
                        move = (touch.position - touchMoveOrigin).normalized;
                        Debug.Log($"touch move");
                    }
                    else
                    {
                        touchMoveOrigin = touch.position;
                    }
                    // todo:
                }
                // ...or flick cursor
                else
                {
                    // todo: 
                }
            }
            else
            {
                // released, reset move origin
                touchMoveOrigin = Vector2.zero;
                move = Vector2.zero;
            }

            debugValues.vector2_0 = touch.position;
            debugValues.vector2_1 = new Vector2(Screen.width, Screen.height);
        }
    }

    // todo: move to top of file and organize
    private Vector2 touchMoveOrigin;

    public void ResetFlick()
    {
        InputFlickVelocity = Vector3.zero;
    }
}
