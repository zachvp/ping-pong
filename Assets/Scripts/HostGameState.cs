using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


// todo: remove
public class HostGameState : CoreSingletonNetworkBehavior<HostGameState>
{
    public Transform[] spawns;
    public Transform center;

    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += (clientID) =>
        {
            if (clientID > 0)
            {
                // todo: spawn ball
                Debug.Log($"players connected; spawn ball");
            }
        };
    }
}
