using System;
using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

// todo: remove
public class HostGameState : CoreSingletonNetworkBehavior<HostGameState>
{
    public Transform[] spawns;
    public Transform center;

    public Action OnGameStart;

    public List<INetworkGameStateHandler> handlers = new();

    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += (clientID) =>
        {
            if (clientID > 0)
            {
                // todo: spawn ball
                Debug.Log($"players connected; spawn ball");
                OnGameStart?.Invoke();
            }
        };
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        OnGameStart = null;
    }

    public void ResetGame()
    {
        foreach (var handler in handlers)
        {
            handler.HandleGameResetClientRpc();
        }

        StartCoroutine(Task.Delayed(0.7f, () => OnGameStart?.Invoke()));
    }

    public void RegisterGameResetHandler(INetworkGameStateHandler handler)
    {
        handlers.Add(handler);
    }
}

public interface INetworkGameStateHandler
{
    [Rpc(SendTo.ClientsAndHost)]
    public void HandleGameResetClientRpc();
}
