using UnityEngine;
using Mirror;

public class PredictedPlayerTriggeredAnimation : NetworkBehaviour
{

    AnimationPriority animationPriority;
    Vector3 moveDirection;
    float animationTime;

    public void TriggerPredictedMovementAnimation(Vector3 moveDirection, float animationTime, AnimationPriority animationPriority)
    {
        if (animationPriority > this.animationPriority)
        {
            this.moveDirection = moveDirection;
            this.animationTime = animationTime;
            this.animationPriority = animationPriority;
        }
    }

    // public override StatePayload ProcessTick(StatePayload statePayload, InputPayload inputPayload)
    // {


    //     return statePayload;
    // }

}

public enum AnimationPriority
{
    Interrupt,
    Attack
}
