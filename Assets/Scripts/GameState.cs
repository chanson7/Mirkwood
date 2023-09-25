using Mirror;
using UnityEngine;

public class GameState : NetworkBehaviour
{

    public enum EGameState { Off, Loading, Ready, Pregame, Play, End, Postgame };

    [SyncVar(hook = nameof(GameStateChanged))] public EGameState currentState;

    [Server]
    public void SetState(EGameState newState)
    {
        if (currentState == newState) return;

        currentState = newState;
    }

    void GameStateChanged(EGameState oldState, EGameState newState)
    {
        Debug.Log($"Game State: {oldState} -> {newState}");
        switch (newState)
        {
            case EGameState.Pregame:
                break;
            case EGameState.Play:
                break;
            case EGameState.End:
                break;
            case EGameState.Postgame:
                break;
            default:
                break;
        }
    }

}
