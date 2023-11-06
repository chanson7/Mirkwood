using UnityEngine;

public class PredictedPlayerHit : PredictedTransformModule, IPredictedStateProcessor
{

    #region EDITOR EXPOSED FIELDS

    [Header("Knockback Settings")]
    [SerializeField]
    [Tooltip("The length of time a knockback lasts (in seconds)")]
    float knockbackDuration = 0.2f;

    [Tooltip("The point at which we consider the knockback finished, needed because a Vector3 magnitude will not reach zero")]
    [SerializeField] float knockbackEndMagnitude = 0.0001f;

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

        statePayload.HitVector += _hitVector;
        _hitVector = Vector3.zero;

        //start Hit
        if (!statePayload.PlayerState.Equals(PlayerState.Hit) && statePayload.HitVector.magnitude > 0)
        {
            statePayload.PlayerState = PlayerState.Hit;
            statePayload.LastStateChangeTick = statePayload.Tick;
        }

        //during Hit
        else if (statePayload.PlayerState.Equals(PlayerState.Hit))
        {
            Vector3 tickKnockback = inputPayload.TickDuration * (statePayload.HitVector / knockbackDuration);

            characterController.Move(tickKnockback);

            statePayload.HitVector -= tickKnockback;
            statePayload.Position = transform.position;

            //end hit
            if(statePayload.HitVector.magnitude <= knockbackEndMagnitude)
            {
                statePayload.HitVector = Vector3.zero;
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
