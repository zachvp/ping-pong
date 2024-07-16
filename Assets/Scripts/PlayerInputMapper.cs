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

    public TouchCursor touchCursor;
    public float touchDeltaThreshold = 1;
    public float touchDeltaJoystickThreshold = 64;
    public bool touchJoystickEnabled;


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

        isHitPressed = input.actions["hit"].WasPressedThisFrame();

        inputFlick = input.actions["flick"].ReadValue<Vector2>();
        InputFlickVelocity = inputFlick - InputFlickVelocity;

        isFlick = InputFlickVelocity.sqrMagnitude > inputFlickVelocityThreshold;
        if (isFlick)
        {
            InputFlickVelocityTriggered = InputFlickVelocity;
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
                    touchJoystickEnabled = true;

                    var deltaFromOrigin = (touch.position - touchMoveOrigin);
                    if (touchMoveOrigin.sqrMagnitude > Mathf.Epsilon &&
                        (touch.delta.sqrMagnitude > touchDeltaThreshold) || deltaFromOrigin.sqrMagnitude > touchDeltaJoystickThreshold)
                    {
                        //move = deltaFromOrigin.normalized;
                        //move = Common.SmoothStep(move);
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

                // adjust cursor
                var cursorOffset = touchCursor.ComputeJoystickOffset(touch.position);
                debugValues.vector2_2 = cursorOffset;
                touchCursor.MoveCursor(touchCursor.ComputeJoystickPosition(cursorOffset));
                move = cursorOffset / touchCursor.joystickRadius;
                //touchCursor.MoveCursor(touch.position);
            }
            else
            {
                // released, reset move origin
                touchJoystickEnabled = true;
                touchCursor.ResetPosition();
                //touchMoveOrigin = Vector2.zero;
                move = Vector2.zero;
                //cursor.gameObject.SetActive(false);
            }

            debugValues.vector2_0 = touch.position;
            debugValues.vector2_1 = touch.delta;
            debugValues.flt = Mathf.Max(touch.delta.sqrMagnitude, debugValues.flt);
        }
    }

    // todo: move to top of file and organize
    private Vector2 touchMoveOrigin;

    public void ResetFlick()
    {
        InputFlickVelocity = Vector3.zero;
    }
}
