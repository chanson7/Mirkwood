using UnityEngine;

public class PredictedPlayerHit : PredictedTransformModule, IPredictedStateProcessor
{
    #region FIELDS
    
    Vector3 _hitVector = Vector3.zero;
    
    #endregion

    #region PROPERTIES
    
    public Vector3 HitVector 
    { 
        private get { return _hitVector; }
        set 
        {
            Debug.Log("I got hit!");
            _hitVector += value;
        }
    }

    #endregion

    public void ProcessTick(ref StatePayload statePayload, InputPayload inputPayload)
    {

        statePayload.HitVector += _hitVector;
        _hitVector = Vector3.zero;

        //start Hit
        if (!statePayload.PlayerState.Equals(PlayerState.Hit) && statePayload.HitVector.magnitude > 0)
        {
            Debug.Log($"Player has been hit! {statePayload.HitVector.magnitude}");
            statePayload.PlayerState = PlayerState.Hit;
            statePayload.LastStateChangeTick = statePayload.Tick;
        }

        //end Hit
        if (statePayload.PlayerState.Equals(PlayerState.Hit))
        {
            HitVector = Vector3.zero;

            statePayload.HitVector = HitVector;
            statePayload.PlayerState = PlayerState.Balanced;
            statePayload.LastStateChangeTick = statePayload.Tick;
        }
    }
    
}
