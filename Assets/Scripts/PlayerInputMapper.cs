using UnityEngine;
using UnityEngine.InputSystem;
using System;
using static Unity.VisualScripting.Member;

public class PlayerInputMapper : MonoBehaviour
{
    public float moveSpeed = 5;

    public float dashMoveSpeedMultiplier = 1.5f;
    public float dashTimeLength = 0.5f;

    public float hitTimeLength = 0.4f;

    private PlayerInput input;

    public Vector3 Velocity { get; private set; }
    public Rigidbody Body { get { return body; } }
    private Rigidbody body;
    private Vector3 positionInitial;
    private Vector3 positionPrevious;

    private Material material;
    private Color initialColor;
    public Color hitColor = Color.red;

    public Vector2 InputFlickVelocity { get; private set; }
    public Vector3 InputFlickVelocityDash { get; private set; }
    public float stationaryFlickMultiplier = 2;

    public State state;// { get; private set; }

    private Vector3 cursorWorldPosition;
    private Vector2 cursorPositionPrevious;
    private Vector2Int cursorPositionSampled;
    public float cursorNeutralThreshold = 0.1f;

    // todo: dbg
    public DebugValues debugValues;

    [Flags]
    public enum State
    {
        NONE = 0,
        HIT = 1 << 0,
        DASH = 1 << 1,
    }

    private void Awake()
    {
        input = GetComponent<PlayerInput>();
        body = GetComponent<Rigidbody>();
        material = GetComponent<MeshRenderer>().material;

        initialColor = material.GetColor("_FillColor"); // todo: fix hardcoding
        positionInitial = body.position;
    }

    private void Update()
    {
        if (!state.HasFlag(State.HIT) && input.actions["hit"].WasPressedThisFrame())
        {
            state |= State.HIT;

            hitColor.a = initialColor.a;
            material.color = hitColor;
            StartCoroutine(Task.Delayed(hitTimeLength, () =>
            {
                material.color = initialColor;
                state &= ~State.HIT;
            }));
        }

        var inputFlick = input.actions["flick"].ReadValue<Vector2>();
        InputFlickVelocity = inputFlick - InputFlickVelocity;
        if (!state.HasFlag(State.DASH))
        {
            if (InputFlickVelocity.sqrMagnitude > 0.25f)
            {
                Dash();
                InputFlickVelocityDash = InputFlickVelocity;
            }
        }

        var cursor = input.actions["cursor-mouse"].ReadValue<Vector2>();

        debugValues.vector2_0 = cursor;
        if (cursorPositionSampled.sqrMagnitude > Mathf.Epsilon)
        {
            var newPosition = body.position;
            var zeroedPos = new Vector3(cursor.x, cursor.y, -input.camera.transform.position.z);
            newPosition = input.camera.ScreenToWorldPoint(zeroedPos);
            newPosition.z = positionInitial.z;
            cursorWorldPosition = newPosition;
            debugValues.vector3 = newPosition;
        }
        cursorPositionSampled = FromFloat(cursor) - FromFloat(cursorPositionPrevious);
        cursorPositionPrevious = cursor;

        //Debug.Log($"FromFloat(cursor) - sampledCursorPosition: {FromFloat(cursor)} - {cursorPositionSampled}");
        debugValues.vector2Int_0 = cursorPositionSampled;
        debugValues.flt = cursorPositionSampled.sqrMagnitude;
    }

    public Vector2Int FromFloat(Vector2 source)
    {
        return new Vector2Int((int)source.x, (int)source.y);
    }

    private void FixedUpdate()
    {
        var move = (Vector3) input.actions["move"].ReadValue<Vector2>();
        debugValues.str = input.currentControlScheme;

        if (input.currentControlScheme.Equals("gamepad"))
        {
            if (state.HasFlag(State.DASH))
            {
                body.velocity = InputFlickVelocityDash * moveSpeed * dashMoveSpeedMultiplier;
            }
            else
            {
                body.velocity = move * moveSpeed;
            }
            Velocity = body.velocity;
        }
        else
        {
            Debug.Log("use non-gamepad control scheme");
            body.position = cursorWorldPosition;
            //Velocity = ;
            body.velocity = (body.position - positionPrevious) / Time.fixedDeltaTime;
        }

        positionPrevious = body.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        var ball = collision.gameObject.GetComponent<Ball>();
        if (ball)
        {
            body.velocity = Vector3.zero;
        }
    }

    private void Dash()
    {
        Debug.Log("DASH");
        state |= State.DASH;

        var newColor = Color.blue;
        newColor.a = initialColor.a;
        material.color = newColor;

        StartCoroutine(Task.Delayed(dashTimeLength, () =>
        {
            material.color = initialColor;
            state &= ~State.DASH;
            InputFlickVelocity = Vector3.zero;
        }));
    }

    // todo: move to shared file
    [Serializable]
    public struct DebugValues
    {
        public Vector2 vector2_0;
        public Vector2 vector2_1;

        public Vector2Int vector2Int_0;

        public Vector3 vector3;

        public string str;

        public float flt;
    }
}
