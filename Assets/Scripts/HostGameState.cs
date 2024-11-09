using System;
using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class HostGameState : CoreSingletonNetworkBehavior<HostGameState>
{
    public Transform[] spawns;
    public Transform center;

    public GameState state;

    public List<INetworkGameStateHandler> handlers = new();

    public NetworkVariable<int> scorePlayer0 = new();
    public NetworkVariable<int> scorePlayer1 = new();

    public int minRequiredPlayers = 2;
    public float restartDelay = 0.7f;

    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += (clientID) =>
        {
            if ((int)clientID + 1 >= minRequiredPlayers)
            {
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

    [Rpc(SendTo.Server)]
    public void AddScoreRpc(int playerID, int points)
    {
        switch (playerID)
        {
            case 0:
                scorePlayer0.Value += points;
                break;
            case 1:
                scorePlayer1.Value += points;
                break;
        }
        UpdateState(GameState.SCORE);
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
    RESET,
    SCORE
}
