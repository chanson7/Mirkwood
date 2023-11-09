using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class PredictedPlayerMeleeAttack : PredictedTransformModule, IPredictedInputRecorder, IPredictedInputProcessor
{

    #region EDITOR EXPOSED FIELDS

    [Header("Attacks")]
    [SerializeField] AttackDefinition firstAttack;
    [SerializeField] AttackDefinition secondAttack;
    [SerializeField] AttackDefinition thirdAttack;

    [Header("Settings")]
    [SerializeField]
    AreaOfEffect meleeAreaOfEffect;

    [Tooltip("Determines how quickly an animation cancels back to the default animation state")]
    [SerializeField]
    float animationCancelRate;

    const string defaultAnimationState = "Base Layer.Walking";

    #endregion

    #region FIELDS

    bool isAttackButtonPressed;
    bool isHitApplied = true;
    CharacterController characterController;
    Animator animator;

    #endregion

    #region INPUT

    void OnAttack(InputValue input)
    {
        isAttackButtonPressed = input.isPressed;
    }

    public void RecordInput(ref InputPayload inputPayload)
    {
        inputPayload.AttackPressed = isAttackButtonPressed;
    }

    #endregion

    public void ProcessInput(ref StatePayload statePayload, InputPayload inputPayload)
    {        
        //Start First Attack
        if (inputPayload.AttackPressed && statePayload.PlayerState.Equals(PlayerState.Balanced))
        {
            isHitApplied = false;

            statePayload.PlayerState = PlayerState.Attack1;
            statePayload.LastStateChangeTick = statePayload.Tick;

            TriggerAttackAnimation(firstAttack.AnimationHash);
        }

        //During First Attack
        if (statePayload.PlayerState.Equals(PlayerState.Attack1))
        {
            float attackCompletionPercentage = (statePayload.Tick - statePayload.LastStateChangeTick) * predictedPlayerTransform.ServerTickMs / firstAttack.AttackDuration;

            //Exit First Attack
            if (!inputPayload.AttackPressed && !isHitApplied)
            {
                CancelAttackAnimation();

                statePayload.PlayerState = PlayerState.Balanced;
                statePayload.LastStateChangeTick = statePayload.Tick;
                return;
            }

            //First Attack Damage Tick
            if (isHitApplied == false && firstAttack.HitApplicationTime <= attackCompletionPercentage)
            {
                ApplyHit(statePayload.Position, firstAttack);
            }

            //End First attack
            if (firstAttack.AttackDuration <= (statePayload.Tick - statePayload.LastStateChangeTick) * predictedPlayerTransform.ServerTickMs)
            {
                //Start second attack
                if (inputPayload.AttackPressed)
                {
                    isHitApplied = false;
                    statePayload.PlayerState = PlayerState.Attack2;
                    TriggerAttackAnimation(secondAttack.AnimationHash);
                }
                else
                    statePayload.PlayerState = PlayerState.Balanced;

                statePayload.LastStateChangeTick = statePayload.Tick;
                return;
            }

            if((firstAttack.LungeStartTime <= attackCompletionPercentage && firstAttack.LungeEndTime >= attackCompletionPercentage))
                characterController.Move(firstAttack.LungeDistance * inputPayload.TickDuration * transform.forward / firstAttack.LungeDuration);

            statePayload.Position = transform.position;
        }

        //During Second Attack
        if (statePayload.PlayerState.Equals(PlayerState.Attack2))
        {
            float attackCompletionPercentage = (statePayload.Tick - statePayload.LastStateChangeTick) * predictedPlayerTransform.ServerTickMs / firstAttack.AttackDuration;

            //Exit Second Attack
            if (!inputPayload.AttackPressed && !isHitApplied)
            {
                statePayload.PlayerState = PlayerState.Balanced;
                statePayload.LastStateChangeTick = statePayload.Tick;

                CancelAttackAnimation();

                return;
            }

            //Second Attack Damage Tick
            if (isHitApplied == false && secondAttack.HitApplicationTime <= (statePayload.Tick - statePayload.LastStateChangeTick) * predictedPlayerTransform.ServerTickMs / secondAttack.AttackDuration)
            {
                ApplyHit(statePayload.Position, secondAttack);
            }

            //End Second attack
            if (secondAttack.AttackDuration <= (statePayload.Tick - statePayload.LastStateChangeTick) * predictedPlayerTransform.ServerTickMs)
            {
                //Start Third attack
                if (inputPayload.AttackPressed)
                {
                    isHitApplied = false;
                    statePayload.PlayerState = PlayerState.Attack3;

                    TriggerAttackAnimation(thirdAttack.AnimationHash);

                }
                else
                    statePayload.PlayerState = PlayerState.Balanced;

                statePayload.LastStateChangeTick = inputPayload.Tick;
                return;
            }

            if ((secondAttack.LungeStartTime <= attackCompletionPercentage && secondAttack.LungeEndTime >= attackCompletionPercentage))
                characterController.Move(secondAttack.LungeDistance * inputPayload.TickDuration * transform.forward / secondAttack.LungeDuration);

            statePayload.Position = transform.position;
        }

        //During Third Attack
        if (statePayload.PlayerState.Equals(PlayerState.Attack3))
        {
            float attackCompletionPercentage = (statePayload.Tick - statePayload.LastStateChangeTick) * predictedPlayerTransform.ServerTickMs / firstAttack.AttackDuration;

            //Exit Third Attack
            if (!inputPayload.AttackPressed && !isHitApplied)
            {
                statePayload.PlayerState = PlayerState.Balanced;
                CancelAttackAnimation();

                return;
            }

            //Third Attack Damage Tick
            if (isHitApplied == false && thirdAttack.HitApplicationTime <= (statePayload.Tick - statePayload.LastStateChangeTick) * predictedPlayerTransform.ServerTickMs / thirdAttack.AttackDuration)
            {
                ApplyHit(statePayload.Position, thirdAttack);
            }

            //End Third attack
            if (thirdAttack.AttackDuration <= (statePayload.Tick - statePayload.LastStateChangeTick) * predictedPlayerTransform.ServerTickMs)
            {
                //Restart from First attack
                if (inputPayload.AttackPressed)
                {
                    statePayload.PlayerState = PlayerState.Attack1;

                    TriggerAttackAnimation(firstAttack.AnimationHash);
                }
                else
                    statePayload.PlayerState = PlayerState.Balanced;

                statePayload.LastStateChangeTick = inputPayload.Tick;
                return;
            }

            if ((thirdAttack.LungeStartTime <= attackCompletionPercentage && thirdAttack.LungeEndTime >= attackCompletionPercentage))
                characterController.Move(thirdAttack.LungeDistance * inputPayload.TickDuration * transform.forward / thirdAttack.LungeDuration);

            statePayload.Position = transform.position;
        }
    }

    void ApplyHit(Vector3 myPosition, AttackDefinition attack)
    {
        foreach (Transform other in meleeAreaOfEffect.Collisions)
        {
            if (other.GetComponent<PredictedPlayerReceiveHit>() != null)
            {
                if(isServer)
                    other.GetComponent<PredictedPlayerReceiveHit>().ServerTriggerHitReceived((other.transform.position - myPosition).normalized * attack.KnockbackMultiplier, attack.StunDuration);
            }
        }

        isHitApplied = true;
    }

    void TriggerAttackAnimation(int attackHash)
    {
        if (isLocalPlayer)
            animator.SetTrigger(attackHash);
        
        else if(isServer)
            RpcTriggerAttackAnimation(attackHash);


        [ClientRpc(includeOwner = false)]
        void RpcTriggerAttackAnimation(int attackHash)
        {
            animator.SetTrigger(attackHash);
        }
    }

    void CancelAttackAnimation()
    {
        if (isLocalPlayer)
            animator.CrossFade(defaultAnimationState, animationCancelRate);

        else if (isServer)
            RpcCancelAttackAnimation();

        [ClientRpc(includeOwner = false)]
        void RpcCancelAttackAnimation()
        {
            animator.CrossFade(defaultAnimationState, animationCancelRate);
        }
    }

    #region MONOBEHAVIOUR

    public void Awake()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
    }

    #endregion

}
