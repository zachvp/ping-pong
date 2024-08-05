using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


// todo: remove
public class HostGameState : CoreSingletonNetworkBehavior<HostGameState>
{
    public Transform[] spawns;
    public Transform center;
}
