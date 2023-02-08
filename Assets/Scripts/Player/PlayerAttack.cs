using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class PlayerAttack : NetworkBehaviour
{
    [SerializeField] Animator animator;
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
        RpcBeginAttackAnimation(); //the server plays the animations on all the other clients
    }

    [ClientRpc(includeOwner = false)] //dont do the animation again for the local player because he already saw it
    void RpcBeginAttackAnimation()
    {
        animator.SetTrigger(attackHash);
    }

}
