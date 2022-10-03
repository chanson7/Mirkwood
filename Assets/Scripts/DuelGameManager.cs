using UnityEngine;
using Mirror;

public class DuelGameManager : NetworkBehaviour
{

    double serverStartTime;
    int playerCount = 0;

    [SerializeField] double timeOutInSeconds = 20;

    public override void OnStartServer()
    {
        serverStartTime = Time.timeAsDouble;
    }

    public void OnPlayerJoinedGameSession()
    {
        playerCount++;
    }

    public void OnPlayerLeftGameSession()
    {
        playerCount--;
        CheckForGameTimeOut();
    }

    //do this when a player leaves the game session to see if we should end the game
    [Server]
    void CheckForGameTimeOut()
    {
        if (playerCount < 1 && Time.time - serverStartTime > timeOutInSeconds)
        {
            Debug.Log("..All players have disconnected.  Ending the game.");
            Application.Quit();
        }
    }
}
