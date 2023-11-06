using UnityEngine;

public class PredictedPlayerHit : PredictedTransformModule, IPredictedStateProcessor
{

    #region EDITOR EXPOSED FIELDS

    [Header("Knockback Settings")]
    [SerializeField]
    [Tooltip("The length of time a knockback lasts (in seconds)")]
    float knockbackDuration = 0.2f;

    #endregion

    #region FIELDS

    Vector3 _hitVector = Vector3.zero;
    CharacterController characterController;
    
    #endregion

    #region PROPERTIES
    
    public Vector3 HitVector 
    { 
        private get { return _hitVector; }
        set 
        {
            _hitVector += value;
        }
    }

    #endregion

    public void ProcessTick(ref StatePayload statePayload, InputPayload inputPayload)
    {

        //start Hit
        if (!statePayload.PlayerState.Equals(PlayerState.Hit) && _hitVector.magnitude > 0)
        {
            Debug.Log($"Player has been hit! {statePayload.HitVector}");

            statePayload.HitVector += _hitVector;
            _hitVector = Vector3.zero;
            statePayload.PlayerState = PlayerState.Hit;
            statePayload.LastStateChangeTick = statePayload.Tick;
        }

        //during Hit
        else if (statePayload.PlayerState.Equals(PlayerState.Hit))
        {
            Vector3 tickKnockback = inputPayload.TickDuration * statePayload.HitVector / knockbackDuration;

            characterController.Move(tickKnockback);

            statePayload.HitVector -= tickKnockback;

            //end hit
            if(statePayload.HitVector.magnitude <= 0f)
            {
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
