using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class GameManager : NetworkBehaviour
{

    [SerializeField] List<PlayerObject> players = new(); //server only?
    GameState gameState;

    public static GameManager Singleton { get; internal set; }

    [Server]
    public bool RegisterPlayerObject(PlayerObject player)
    {
        if (players.Contains(player))
        {  
            return false; 
        }
        else
        {
            Debug.Log($"{player} registered with Game Manager");
            players.Add(player);
            
            return true;
        }
    }

    bool InitializeSingleton()
    {
        if (Singleton != null)
        {
            if (Singleton == this) return true;

            Debug.LogWarning("Multiple Game Managers detected in the scene. Only one Game Manager can exist at a time. The duplicate Game Manager will be destroyed.");
            Destroy(gameObject);

            // Return false to not allow collision-destroyed second instance to continue.
            return false;
        }

        Singleton = this;

        return true;
    }

    public override void OnStartServer()
    {

    }

    private void Awake()
    {
        if (!InitializeSingleton()) return;

        gameState = GetComponent<GameState>();
    }
    
}
