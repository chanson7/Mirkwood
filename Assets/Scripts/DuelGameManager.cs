using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System.Linq;

public class DuelGameManager : NetworkBehaviour
{

    enum EGameMode { Solo, Duo };

    [SerializeField] EGameMode gameMode = EGameMode.Solo;
    [SerializeField] Dictionary<int, CombatantDuelist> duelists = new();
    
    GameState gameState;

    public static DuelGameManager Singleton { get; internal set; }

    [Server]
    public bool RegisterDuelist(CombatantDuelist duelist)
    {
        switch(gameMode)
        {
            case EGameMode.Solo:
                if (duelists.ContainsValue(duelist))
                {
                    return false; 
                }
                else
                {
                    int teamNumber = 0;

                    while (duelists.ContainsKey(teamNumber))
                        teamNumber++;
                    
                    duelists.Add(teamNumber ,duelist);

                    Debug.Log($"{duelist} registered with the Game Manager on Team {teamNumber}");

                    return true;
                }
            case EGameMode.Duo:
                return false;
            default:
                return false;
        }
    }

    [Server]
    public CombatantDuelist CycleTarget(CombatantDuelist duelist, CombatantDuelist currentTarget = null)
    {
        CombatantDuelist opponent = currentTarget;
        float distanceToCurrentTarget = currentTarget == null ? Mathf.Infinity : Vector3.Distance(duelist.transform.position, currentTarget.transform.position);
        
        if (duelists.ContainsValue(duelist))
        {
            int friendlyTeam = duelists.First(entry => entry.Value == duelist).Key;

            foreach (KeyValuePair<int, CombatantDuelist> entry in duelists)
            {
                float distanceToNewDuelist = Vector3.Distance(duelist.transform.position, entry.Value.transform.position);

                if (entry.Key != friendlyTeam &&                        //not on our team
                    entry.Value != currentTarget &&                     //not the current target
                    distanceToNewDuelist <= distanceToCurrentTarget)    //closer than the last one
                {
                    opponent = entry.Value;
                }
            }
        }

        return opponent;
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
