using System;
using System.Collections.Generic;
using TMPro;

public class UIDebug : CoreSingletonBehavior<UIDebug>
{
    public TextMeshProUGUI ui;
    public List<Entry> entries = new();

    public void Update()
    {
        Refresh();
    }

    public void OnDrawGizmos()
    {
        //UnityEditor.Handles.Label()
    }

    public void Register(string label, Func<object> getLatestValue)
    {
        var entry = new Entry()
        {
            name = label,
            value = getLatestValue
        };
        entries.Add(entry);
    }

    public void Register(string label, object value)
    {
        var entry = new Entry()
        {
            name = label,
            value = () => value
        };
        entries.Add(entry);
    }

    public void Remove(string name)
    {
        Entry removeEntry = null;

        foreach (var entry in entries)
        {
            if (entry.name.Equals(name))
            {
                removeEntry = entry;
            }
        }

        entries.Remove(removeEntry);
    }

    public void Refresh()
    {
        var text = "";
        foreach (var entry in entries)
        {
            text += $"{entry.name}: {entry.value()}\n";
        }

        ui.text = text;
    }

    public class Entry
    {
        public string name;
        public Func<object> value;
    }
}