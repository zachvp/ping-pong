using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class PlayerInputMapper : MonoBehaviour
{
    private PlayerInput input;

    public bool isHitPressed;
    public bool isDashPressed;

    public Vector2 flick;
    public Vector2 FlickVelocity { get; private set; }
    public float flickVelocityThreshold = 0.25f;
    //public Vector3 FlickVelocityTriggered { get; private set; } // todo: remove

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

        Debug.Assert(input != null, $"player input ref is null");
    }

    private void Update()
    {
        debugValues.str_0 = input.currentControlScheme;
        debugValues.str_1 = input.currentActionMap.name;

        //Debug.LogFormat($"input.currentControlScheme: {input.currentControlScheme}");

        // hit input
        isHitPressed = input.actions["hit"].WasPressedThisFrame();

        //  flick input
        flick = input.actions["flick"].ReadValue<Vector2>();

        // standard move input
        move = input.actions["move"].ReadValue<Vector2>();

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
            else
            {
                // released, reset
                touchJoystickLeft.ResetPosition();
                move = Vector2.zero;
            }

            // touch flick joystick
            if (touchFlick.isInProgress)
            {
                if (touchJoystickRightEnabled)
                {
                    // adjust and respond to cursor
                    var joystickRaw = touchJoystickRight.JoystickRaw(touchFlick.position);
                    debugValues.vector2_2 = joystickRaw;
                    touchJoystickRight.MoveCursor(touchJoystickRight.JoystickScreenPosition(joystickRaw));

                    flick = touchJoystickRight.JoystickNormalized(joystickRaw);
                }
                else
                {
                    touchJoystickRight.anchor.position = touchFlick.position;
                    touchJoystickRightEnabled = true;
                    touchJoystickRight.gameObject.SetActive(true);
                    Debug.Log($"set anchor position");
                }
            }
            else
            {
                touchJoystickRight.ResetPosition();
                touchJoystickRight.gameObject.SetActive(false);
                touchJoystickRightEnabled = false;
                flick = Vector2.zero;
                FlickVelocity = Vector2.zero;
            }
        }
        else
        {
            touchJoystickLeft.gameObject.SetActive(false);
            touchJoystickRight.gameObject.SetActive(false);
        }

        // adjust movement x-axis according to camera direction
        move.x *= input.camera.transform.forward.z;

        // dash button input
        // dash input
        isDashPressed = input.actions["dash"].WasPressedThisFrame();

        // compute flick input to trigger flick gesture
        var resolvedFlickVelocity = flick - FlickVelocity;
        resolvedFlickVelocity.x *= input.camera.transform.forward.z;

        FlickVelocity = resolvedFlickVelocity;
        isFlick = FlickVelocity.sqrMagnitude > flickVelocityThreshold;

        if (isFlick)
        {
            debugValues.vector2_0 = FlickVelocity;
            //FlickVelocityTriggered = FlickVelocity;
        }
    }

    public void ResetFlick()
    {
        FlickVelocity = Vector3.zero;
    }
}
