using UnityEngine;
using Mirror;

public class AIDuelist : CombatantDuelist
{

    enum AIState { Waiting, SelectTarget, ApproachTarget, AttackTarget, DodgeAttack, BlockAttack, Retreat };

    AIState aiState = AIState.Waiting;
    
    GameObject target;

    [Server]
    void SelectTarget()
    {
        
    }

    private void Update()
    {
        if (isServer)
        {
            switch (aiState)
            {
                case AIState.SelectTarget:
                    SelectTarget();
                    break; 
                case AIState.ApproachTarget:
                    break;
                case AIState.AttackTarget:
                    break;
                case AIState.DodgeAttack:
                    break;
                case AIState.BlockAttack:
                    break;
                default:
                    return;
            }
        }
    }

}
