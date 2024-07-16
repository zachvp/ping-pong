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

    // todo: move to shared class
    public static Vector2Int FromFloat(Vector2 source)
    {
        return new Vector2Int((int)source.x, (int)source.y);
    }

    public static Vector3 Round(Vector3 source)
    {
        return new Vector3(Mathf.Round(source.x), Mathf.Round(source.y), Mathf.Round(source.z));
    }

    public static int SignMultiply(float value)
    {
        return value < 0 ? -1 : 1;
    }

    public static Vector3 SignMultiply(Vector3 source, Vector3 multiplier)
    {
        return Vector3.Scale(source, Round(multiplier.normalized));
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

    // todo: override '=' operator
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
}


