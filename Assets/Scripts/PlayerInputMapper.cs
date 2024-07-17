using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.LowLevel;
using static UnityEngine.GraphicsBuffer;

public class PlayerInputMapper : MonoBehaviour
{
    private PlayerInput input;

    public bool isHitPressed;

    // todo: remove 'input' from name prefixes
    public Vector2 inputFlick;
    public Vector2 InputFlickVelocity { get; private set; }
    public float inputFlickVelocityThreshold = 0.25f;
    public Vector3 InputFlickVelocityTriggered { get; private set; }

    public bool isFlick;

    public Vector2 move;

    public TouchCursor touchJoystickLeft;
    public TouchCursor touchJoystickRight;

    // todo: dbg
    public DebugValues debugValues;

    private void Awake()
    {
        input = GetComponent<PlayerInput>();

        input.camera = GameObject.FindGameObjectWithTag(Constants.Tags.CAMERA).GetComponent<Camera>();
    }

    private void Update()
    {
        debugValues.str_0 = input.currentControlScheme;
        debugValues.str_1 = input.currentActionMap.name;

        // hit input
        isHitPressed = input.actions["hit"].WasPressedThisFrame();

        //  flick input
        inputFlick = input.actions["flick"].ReadValue<Vector2>();

        // standard move input
        move = input.actions["move"].ReadValue<Vector2>();  // todo: rename to cursor0, cursor1

        // touchscreen input overrides
        if (input.currentControlScheme.Equals("touchscreen"))
        {
            var tap = input.actions["touch.tap"];
            isHitPressed = tap.WasPressedThisFrame();
            if (isHitPressed)
            {
                Debug.Log("tap detected");
            }

            // touch move joystick
            var touchMove = input.actions["touch.move"].ReadValue<TouchState>();
            if (touchMove.isInProgress)
            {
                // check for a move cursor...
                if (touchMove.position.x < Screen.width / 2)
                {
                    ProcessTouchJoystick(touchMove, touchJoystickLeft);
                }
            }
            else
            {
                // released, reset
                touchJoystickLeft.ResetPosition();
                move = Vector2.zero;
            }

            // touch flick joystick
            var touchFlick = input.actions["touch.flick"].ReadValue<TouchState>();
            if (touchFlick.isInProgress)
            {
                if (touchFlick.position.x > Screen.width / 2)
                {
                    touchJoystickRight.gameObject.SetActive(true);

                    // adjust and respond to cursor
                    var joystickRaw = touchJoystickRight.JoystickRaw(touchFlick.position);
                    debugValues.vector2_2 = joystickRaw;
                    touchJoystickRight.MoveCursor(touchJoystickRight.JoystickScreenPosition(joystickRaw));

                    var joystickNormalized = touchJoystickRight.JoystickNormalized(joystickRaw);
                    if (Mathf.Abs(joystickNormalized.x) > touchJoystickRight.joystickDeadzone ||
                        Mathf.Abs(joystickNormalized.y) > touchJoystickRight.joystickDeadzone)
                    {
                        inputFlick = joystickNormalized;
                        debugValues.vector2_0 = joystickNormalized;
                    }
                }
            }
            else
            {
                touchJoystickRight.ResetPosition();
                inputFlick = Vector2.zero;
                InputFlickVelocity = Vector2.zero;
            }
        }
        else
        {
            touchJoystickLeft.gameObject.SetActive(false);
            touchJoystickRight.gameObject.SetActive(false);
        }

        // compute flick input to trigger flick gesture
        InputFlickVelocity = inputFlick - InputFlickVelocity;
        isFlick = InputFlickVelocity.sqrMagnitude > inputFlickVelocityThreshold;
        if (isFlick)
        {
            InputFlickVelocityTriggered = InputFlickVelocity;
        }
    }

    private void ProcessTouchJoystick(TouchState touch, TouchCursor target)
    {
        target.gameObject.SetActive(true);

        // adjust and respond to cursor
        var joystickRaw = target.JoystickRaw(touch.position);
        debugValues.vector2_2 = joystickRaw;
        target.MoveCursor(target.JoystickScreenPosition(joystickRaw));

        var joystickNormalized = target.JoystickNormalized(joystickRaw);
        if (Mathf.Abs(joystickNormalized.x) > target.joystickDeadzone ||
            Mathf.Abs(joystickNormalized.y) > target.joystickDeadzone)
        {
            move = joystickNormalized;
        }
    }

    public void ResetFlick()
    {
        InputFlickVelocity = Vector3.zero;
    }
}
