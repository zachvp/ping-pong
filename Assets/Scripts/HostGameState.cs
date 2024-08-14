using System;
using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

// todo: remove
public class HostGameState : CoreSingletonNetworkBehavior<HostGameState>
{
    public Transform[] spawns;
    public Transform center;

    public GameState state;

    public List<INetworkGameStateHandler> handlers = new();

    public int minRequiredPlayers = 2;
    public float restartDelay = 0.7f;

    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += (clientID) =>
        {
            if ((int)clientID + 1 >= minRequiredPlayers)
            {
                // todo: spawn ball
                Debug.Log($"players connected; spawn ball");
                StartGame();
            }
        };
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }

    public void StartGame()
    {
        UpdateState(GameState.MAIN);
    }

    public void ResetGame()
    {
        UpdateState(GameState.RESET);

        StartCoroutine(Task.Delayed(restartDelay, () => StartGame()));
    }

    private void UpdateState(GameState current)
    {
        var old = state;
        state = current;
        foreach (var handler in handlers)
        {
            handler.HandleGameStateChangeRpc(old, state);
        }
    }

    public void RegisterGameResetHandler(INetworkGameStateHandler handler)
    {
        handlers.Add(handler);
    }
}

public interface INetworkGameStateHandler
{
    [Rpc(SendTo.ClientsAndHost)]
    public void HandleGameStateChangeRpc(GameState old, GameState current);
}

public enum GameState
{
    NONE,
    MAIN,
    RESET
}
