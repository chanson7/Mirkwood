using Mirror;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PredictedPlayerDisable : PredictedTransformModule, IPredictedInputProcessor
{

    #region FIELDS

    CharacterController characterController;

    #endregion

    [Server]
    public void ServerTriggerInterrupt(Vector3 knockback, float duration)
    {
        predictedPlayerTransform.EnqueueUnpredictedEvent(new UnpredictedTransformEffect
        {
            Translation = knockback,
            Duration = duration
        });
    }

    public void ProcessInput(ref StatePayload statePayload, InputPayload input)
    {
        //first interrupt tick
        if (!statePayload.PlayerState.Equals(PlayerState.Disabled) && statePayload.effectDisable > 0f)
        {
            Debug.Log($"disabled on tick: {statePayload.Tick}");
            statePayload.PlayerState = PlayerState.Disabled;
            statePayload.LastStateChangeTick = statePayload.Tick;
        }

        //during interrupt
        if (statePayload.PlayerState.Equals(PlayerState.Disabled))
        {
            Vector3 tickKnockback = input.TickDuration * (statePayload.effectTranslate / statePayload.effectDisable);

            characterController.Move(tickKnockback);

            statePayload.effectTranslate -= tickKnockback;
            statePayload.effectDisable -= input.TickDuration;
            statePayload.Position = transform.position;

            //end interrupt
            if(statePayload.effectDisable <= 0f)
            {
                statePayload.effectDisable = 0f;
                statePayload.effectTranslate = Vector3.zero;
                statePayload.PlayerState = PlayerState.Balanced;
                statePayload.LastStateChangeTick = statePayload.Tick;
            }
        }
    }

    #region MONOBEHAVIOUR

    public void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    #endregion

}

public struct InterruptPayload
{
    public bool isActive;
    public float TickDuration;
    public float RemainingDuration;
    public Vector3 KnockbackDistance;
}
