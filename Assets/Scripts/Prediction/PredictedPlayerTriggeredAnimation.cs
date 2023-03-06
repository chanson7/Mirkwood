using UnityEngine;
using Mirror;

public class PredictedPlayerTriggeredAnimation : PredictedPlayerTickProcessor
{

    AnimationPriority animationPriority;

    public void TriggerPredictedMovementAnimation()
    {

    }

    public override InputPayload GatherInput(InputPayload inputPayload)
    {

        return inputPayload;
    }

    public override StatePayload ProcessTick(StatePayload statePayload, InputPayload inputPayload)
    {

        return statePayload;
    }

}
