using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class PlayerAttack : NetworkBehaviour
{
    [SerializeField] Animator animator;
    NetworkAnimator networkAnimator;
    static int attackHash = Animator.StringToHash("Attack");

    void OnAttack(InputValue input)
    {
        BeginAttackAnimation();
    }

    void BeginAttackAnimation()
    {
        animator.SetTrigger(attackHash); //the animation happens immediately for the local player
        CmdBeginAttackAnimation(); //tell the server to do the animation too
    }

    [Command]
    void CmdBeginAttackAnimation()
    {
        animator.SetTrigger(attackHash); //the animation needs to happen on the server if there is root motion
        RpcBeginAttackAnimation(); //the server plays the animations on all the other clients
    }

    [ClientRpc(includeOwner = false)] //dont do the animation again for the local player because he already saw it
    void RpcBeginAttackAnimation()
    {
        animator.SetTrigger(attackHash);
    }

}
