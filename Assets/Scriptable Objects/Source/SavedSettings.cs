using System;
using UnityEngine;

[CreateAssetMenu(
    fileName = nameof(SavedSettings),
    menuName = "ScriptableObject/" + nameof(SavedSettings))]
public class SavedSettings : ScriptableObject
{
    public string ipAddress;
    public string hostedGameName;

#if DEBUG
    public IPEntry[] ipEntries;

    [Serializable]
    public struct IPEntry
    {
        public string label;
        public string address;
    }
#endif
}