using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class PredictedPlayerAttack : PredictedTransformModule, IPredictedInputRecorder, IPredictedStateProcessor
{

    bool attackButtonPressedThisTick;

    void OnAttack(InputValue input)
    {
        attackButtonPressedThisTick = input.isPressed;
    }

    public void RecordInput(ref InputPayload inputPayload)
    {
        inputPayload.AttackPressed = attackButtonPressedThisTick;
        attackButtonPressedThisTick = false;
    }

    public void ProcessTick(ref StatePayload statePayload, InputPayload inputPayload)
    {
        if (statePayload.PlayerState.Equals(PlayerState.Upright) && inputPayload.AttackPressed)
        {
            Debug.Log($"Attack started at {NetworkTime.time} for tick {statePayload.Tick}");
            if(isLocalPlayer)
                Debug.Log($"estimate of network time: {NetworkTime.time + (NetworkTime.rtt / 2f)} for tick {statePayload.Tick}");
            //statePayload.PlayerState = PlayerState.Attacking;
        }
        else return;
        
    }

}
