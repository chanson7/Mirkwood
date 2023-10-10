using UnityEngine;

public class PredictedPlayerHit : PredictedTransformModule, IPredictedStateProcessor
{
    
    public void ProcessTick(ref StatePayload statePayload, InputPayload inputPayload)
    {
        if (!statePayload.PlayerState.Equals(PlayerState.Hit) && statePayload.HitVector.magnitude > 0)
        {
            Debug.Log($"Player has been hit! {statePayload.HitVector.magnitude}");
            statePayload.PlayerState = PlayerState.Hit;
            statePayload.LastStateChangeTick = statePayload.Tick;
        }

        if (statePayload.PlayerState.Equals(PlayerState.Hit))
        {
            statePayload.HitVector = Vector3.zero;
            statePayload.PlayerState = PlayerState.Balanced;
            statePayload.LastStateChangeTick = statePayload.Tick;
        }

    }
}
