using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{

    double serverStartTime;
    int playerCount = 0;
    GameLiftServer gameLiftServer;

    [SerializeField] double timeOutInSeconds = 30;

    public override void OnStartServer()
    {
        serverStartTime = Time.timeAsDouble;
    }

    //do this when a player disconnects to see if we should end the game
    void CheckForGameTimeOut()
    {
        if (playerCount == 0 && Time.time - serverStartTime > timeOutInSeconds)
        {
            Debug.Log("..All players have disconnected.  Ending the game.");
        }
    }
}
