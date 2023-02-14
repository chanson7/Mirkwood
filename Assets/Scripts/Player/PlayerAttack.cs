using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class PlayerAttack : NetworkBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Animator rootMotionAnimator;
    [SerializeField] MeleeCollision meleeCollision;
    [SerializeField] PlayerEnergy playerEnergy;
    [SerializeField] uint attackEnergyCost;
    static int attackHash = Animator.StringToHash("Attack");

    void OnAttack(InputValue input)
    {
        if (playerEnergy.GetEnergy() >= attackEnergyCost)
            AnimateAttack();
    }

    void AnimateAttack()
    {
        animator.SetTrigger(attackHash); //the animation happens immediately for the local player
        rootMotionAnimator.SetTrigger(attackHash);

        CmdAnimateAttack(); //tell the server to do the animation too
    }

    [Command]
    void CmdAnimateAttack()
    {
        if (playerEnergy.SpendEnergy(attackEnergyCost)) //if the player has enough energy
        {
            animator.SetTrigger(attackHash);
            RpcAnimateAttack(); //the server plays the animations on all the other clients
        }
    }

    [ClientRpc(includeOwner = false)] //dont do the animation again for the local player because he already saw it
    void RpcAnimateAttack()
    {
        animator.SetTrigger(attackHash);
    }

    void DealDamage(float angularDifferenceFromForwardVector)
    {
        if (isServer)
        {
            foreach (Collider collider in meleeCollision.damageableColliders)
            {
                Vector3 directionToOtherTransform = collider.transform.position - this.transform.position;
                float angle = Vector3.Angle(transform.forward, directionToOtherTransform);

                if (angle < angularDifferenceFromForwardVector)
                {
                    Debug.Log($"..Deal damage to {collider.name} :O");
                }

            }
        }

    }

}
