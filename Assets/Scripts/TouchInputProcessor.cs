using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class TouchInputProcessor : InputProcessor<TouchState>
{
#if UNITY_EDITOR
    static TouchInputProcessor()
    {
        Initialize();
    }
#endif

    public Direction2D screenRegion;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize()
    {
        InputSystem.RegisterProcessor<TouchInputProcessor>();
    }

    public override TouchState Process(TouchState value, InputControl control)
    {
        var processed = value;

        if (screenRegion.HasFlag(Direction2D.LEFT))
        {
            if (value.position.x > Screen.width / 2)
            {
                processed.phase = UnityEngine.InputSystem.TouchPhase.None;
                // todo: change other data?
            }
        }

        return processed;
    }
}
