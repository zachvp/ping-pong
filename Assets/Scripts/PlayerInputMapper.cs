using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

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
    private bool touchJoystickRightEnabled;

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

            // touch joysticks
            var touch0 = input.actions["touch.0"].ReadValue<TouchState>();
            var touch1 = input.actions["touch.1"].ReadValue<TouchState>();
            var touchMove = touch0;
            var touchFlick = touch0;

            if (touch0.isInProgress)
            {
                if (touch0.position.x > Screen.width / 2)
                {
                    if (!touch1.isInProgress)
                        touchMove.phase = UnityEngine.InputSystem.TouchPhase.None;
                }
                else
                {
                    if (!touch1.isInProgress)
                        touchFlick.phase = UnityEngine.InputSystem.TouchPhase.None;
                }
            }
            if (touch1.isInProgress)
            {
                if (touch1.position.x > Screen.width / 2)
                {
                    touchFlick = touch1;
                    if (!touch0.isInProgress)
                        touchMove.phase = UnityEngine.InputSystem.TouchPhase.None;
                }
                else
                {
                    touchMove = touch1;
                    if (!touch0.isInProgress)
                        touchFlick.phase = UnityEngine.InputSystem.TouchPhase.None;
                }
            }

            if (touchMove.isInProgress)
            {
                // check for a move cursor...
                //if (touchMove.position.x < Screen.width / 2)
                {
                    touchJoystickLeft.gameObject.SetActive(true);

                    // adjust and respond to cursor
                    var joystickRaw = touchJoystickLeft.JoystickRaw(touchMove.position);
                    debugValues.vector2_2 = joystickRaw;
                    touchJoystickLeft.MoveCursor(touchJoystickLeft.JoystickScreenPosition(joystickRaw));

                    var joystickNormalized = touchJoystickLeft.JoystickNormalized(joystickRaw);
                    if (Mathf.Abs(joystickNormalized.x) > touchJoystickLeft.joystickDeadzone ||
                        Mathf.Abs(joystickNormalized.y) > touchJoystickLeft.joystickDeadzone)
                    {
                        move = joystickNormalized;
                    }
                }
            }
            else
            {
                // released, reset
                touchJoystickLeft.ResetPosition();
                move = Vector2.zero;
            }

            // touch flick joystick
            if (touchFlick.isInProgress)
            {
                //if (touchFlick.position.x > Screen.width / 2)
                {
                    if (touchJoystickRightEnabled)
                    {
                        // adjust and respond to cursor
                        var joystickRaw = touchJoystickRight.JoystickRaw(touchFlick.position);
                        debugValues.vector2_2 = joystickRaw;
                        touchJoystickRight.MoveCursor(touchJoystickRight.JoystickScreenPosition(joystickRaw));

                        inputFlick = touchJoystickRight.JoystickNormalized(joystickRaw);
                    }
                    else
                    {
                        touchJoystickRight.anchor.position = touchFlick.position;
                        touchJoystickRightEnabled = true;
                        touchJoystickRight.gameObject.SetActive(true);
                        Debug.Log($"set anchor position");
                    }
                }
            }
            else
            {
                touchJoystickRight.ResetPosition();
                touchJoystickRight.gameObject.SetActive(false);
                touchJoystickRightEnabled = false;
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

    public void ResetFlick()
    {
        InputFlickVelocity = Vector3.zero;
    }
}
