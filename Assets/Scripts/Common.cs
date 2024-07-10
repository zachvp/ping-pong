using System;
using UnityEngine;

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
