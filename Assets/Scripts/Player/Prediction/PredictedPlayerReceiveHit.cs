using Mirror;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PredictedPlayerReceiveHit : PredictionModule, IPredictedInputProcessor
{

    #region FIELDS

    CharacterController characterController;
    Animator animator;

    static readonly int hitForwardHash = Animator.StringToHash("HitForward");
    static readonly int hitRightHash = Animator.StringToHash("HitRight");
    static readonly int hitReceivedHash = Animator.StringToHash("ReceivedHit");

    #endregion

    [Server]
    public void ServerTriggerHitReceived(Vector3 knockback, float duration)
    {
        predictedCharacterController.EnqueueUnpredictedEvent(new UnpredictedEvent
        {
            Translation = knockback,
            Duration = duration
        });
    }

    public void ProcessInput(ref StatePayload statePayload, InputPayload input)
    {
        //first interrupt tick
        if ((!statePayload.PlayerState.Equals(PlayerState.Disabled) && !statePayload.PlayerState.Equals(PlayerState.Blocking)) && statePayload.effectDuration > 0f)
        {
            statePayload.PlayerState = PlayerState.Disabled;
            statePayload.LastStateChangeTick = statePayload.Tick;

            if (isLocalPlayer)
                TriggerHitAnimation(statePayload.effectTranslate.normalized);
            if (isServer)
                RpcTriggerHitAnimation(statePayload.effectTranslate.normalized);
        }

        //during interrupt
        if (statePayload.PlayerState.Equals(PlayerState.Disabled))
        {
            Vector3 tickKnockback = input.TickDuration * (statePayload.effectTranslate / statePayload.effectDuration);
            tickKnockback.y = 0f; //dont get knocked up into the air

            characterController.Move(tickKnockback);

            statePayload.effectTranslate -= tickKnockback;
            statePayload.effectDuration -= input.TickDuration;
            statePayload.Position = transform.position;

            //end interrupt
            if(statePayload.effectDuration <= 0f)
            {
                statePayload.effectDuration = 0f;
                statePayload.effectTranslate = Vector3.zero;
                statePayload.PlayerState = PlayerState.Balanced;
                statePayload.LastStateChangeTick = statePayload.Tick;
            }
        }
    }

    void TriggerHitAnimation(Vector3 hitDirection)
    {
        animator.SetFloat(hitForwardHash, hitDirection.z);
        animator.SetFloat(hitRightHash, hitDirection.x);
        animator.SetTrigger(hitReceivedHash);
    }

    [ClientRpc(includeOwner = false)]
    void RpcTriggerHitAnimation(Vector3 hitDirection)
    {
        animator.SetFloat(hitForwardHash, hitDirection.z);
        animator.SetFloat(hitRightHash, hitDirection.x);
        animator.SetTrigger(hitReceivedHash);
    }

    #region MONOBEHAVIOUR

    public void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    #endregion

}
