using UnityEngine;
using Mirror;

public class AIDuelist : CombatantDuelist
{

    enum AIState { Waiting, FindTarget, ApproachTarget, AttackTarget, DodgeAttack, BlockAttack, Retreat };

    [SerializeField] float desiredDistanceToTarget = 1f;
    [SerializeField] float desiredDistanceLeash = 1f;
    [SerializeField] float rotationSpeed = 1f;

    AIState state = AIState.FindTarget;
    Transform target;

    [Server]
    void FindNextTarget()
    {
        target = DuelGameManager.Singleton.CycleTarget(this).transform;

        if (target != null)
        {
            Debug.Log($"New Target Found.. {target.name}");
            state = AIState.ApproachTarget;
        }
    }

    [Server]
    void ApproachTarget(Transform target)
    {
        float currentDistanceToTarget = Vector3.Distance(transform.position, target.position);
        float angle = Vector3.SignedAngle(transform.forward, target.position - transform.position, Vector3.up);

        //looking the wrong way
        if (Mathf.Abs(angle) > 0f) {
            rotation.RotationXInput = angle * rotationSpeed;
            //movement.MovementInput = Vector2.right * Mathf.Sign(angle);
        }
        else
        {
            //too far away
            if (currentDistanceToTarget > desiredDistanceToTarget + desiredDistanceLeash)
            {
                movement.MovementInput = Vector2.up;
            }
            //too close
            else if (currentDistanceToTarget < desiredDistanceToTarget - desiredDistanceLeash)
            {
                movement.MovementInput = Vector2.down;
            } else
            {
                movement.MovementInput = Vector2.zero;
            }
        }
    }

    private void Update()
    {
        if (isServer)
        {
            switch (state)
            {
                case AIState.FindTarget:
                    FindNextTarget();
                    break; 
                case AIState.ApproachTarget:
                    ApproachTarget(target);
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
