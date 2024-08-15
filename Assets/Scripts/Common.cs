using System;
using System.Collections;
using UnityEngine;

public static class Common
{
    public static void StopNullableCoroutine(MonoBehaviour behavior, IEnumerator enumerator)
    {
        if (enumerator != null)
        {
            behavior.StopCoroutine(enumerator);
        }
    }

    public static Vector2Int FromFloat(Vector2 source)
    {
        return new Vector2Int((int)source.x, (int)source.y);
    }

    public static Vector3 Round(Vector3 source)
    {
        return new Vector3(Mathf.Round(source.x), Mathf.Round(source.y), Mathf.Round(source.z));
    }

    public static Vector2 SmoothStep(Vector2 source)
    {
        var result = Vector2.zero;

        if (source.x > 0)
        {
            result.x = Mathf.SmoothStep(0, 1, source.x);
        }
        else
        {
            result.x = Mathf.SmoothStep(-1, 0, source.x);
        }

        if (source.y > 0)
        {
            result.y = Mathf.SmoothStep(0, 1, source.y);
        }
        else
        {
            result.y = Mathf.SmoothStep(-1, 0, source.y);
        }

        return result;
    }

    public static int SignMultiply(float value)
    {
        return value < 0 ? -1 : 1;
    }

    public static Vector3 SignMultiply(Vector3 source, Vector3 multiplier)
    {
        return Vector3.Scale(source, Round(multiplier.normalized));
    }

    public static Vector3 SignMultiply(Vector3 source, float multiplier)
    {
        return source * Mathf.Round(multiplier);
    }
}

[Serializable]
public struct DebugValues
{
    public Vector2 vector2_0;
    public Vector2 vector2_1;
    public Vector2 vector2_2;

    public Vector2Int vector2Int_0;

    public Vector3 vector3;

    public string str_0;
    public string str_1;

    public float flt;

    public bool bool_0;
    public bool bool_1;
}

[AttributeUsage(AttributeTargets.Field)]
public class CoreConditionalAttribute : PropertyAttribute
{
    public readonly string sourcePropertyName;
    public readonly object checkValue;

    public CoreConditionalAttribute(string sourceName)
    {
        sourcePropertyName = sourceName;
    }

    public CoreConditionalAttribute(string sourceName, object valueToCompare)
        : this(sourceName)
    {
        checkValue = valueToCompare;
    }
}

[Serializable]
public struct SharedDataWrapper
{
    [CoreConditional(nameof(dataLocal), null)]
    public DataAsset dataGlobal;

    [CoreConditional(nameof(dataGlobal), null)]
    public DataInstance dataLocal;
}

[Serializable]
public struct VarWatch<T> where T : struct
{
    public T Value { get => value; }
    public T InitialValue { get => initialValue; }
    public T OldValue { get; private set; }

    [SerializeField]
    private T value;

    [SerializeField]
    private T initialValue;

    // Change event that sends (oldValue, newValue)
    public Action<T, T> onChanged;
    public Action<T, T> onSet;

    public void Set(T newValue)
    {
        value = newValue;

        onSet?.Invoke(OldValue, newValue);

        if (!OldValue.Equals(newValue))
        {
            onChanged?.Invoke(OldValue, newValue);
            OldValue = value;
        }
    }

    public void Reset()
    {
        value = initialValue;
        onChanged = null;
        onSet = null;
    }

    public override string ToString()
    {
        return value.ToString();
    }
}

public static class Constants
{
    public const float FRAME_TIME = 1f / 60f;

    public static class Tags
    {
        public const string CAMERA = "MainCamera";
        public const string CURSOR = "Cursor";
    }
}

[Flags]
public enum Direction2D
{
    NONE = 0,
    LEFT = 1 << 1,
    RIGHT = 1 << 2,
    DOWN = 1 << 3,
    UP = 1 << 4
}
