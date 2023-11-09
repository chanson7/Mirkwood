using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class PredictedPlayerMeleeAttack : PredictedTransformModule, IPredictedInputRecorder, IPredictedInputProcessor
{

    #region EDITOR EXPOSED FIELDS

    [Header("Attacks")]
    [SerializeField] AttackDefinition primaryAttack;
    [SerializeField] AttackDefinition secondaryAttack;
    [SerializeField] AttackDefinition tertiaryAttack;

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
        //Start Primary Attack
        if (inputPayload.AttackPressed && statePayload.PlayerState.Equals(PlayerState.Balanced))
        {
            isHitApplied = false;

            statePayload.PlayerState = PlayerState.Attacking_Primary;
            statePayload.LastStateChangeTick = statePayload.Tick;

            TriggerAttackAnimation(primaryAttack.AnimationHash);
        }

        //During Primary Attack
        if (statePayload.PlayerState.Equals(PlayerState.Attacking_Primary))
        {
            float attackCompletionPercentage = (statePayload.Tick - statePayload.LastStateChangeTick) * predictedPlayerTransform.ServerTickMs / primaryAttack.AttackDuration;

            //Exit Primary Attack
            if (!inputPayload.AttackPressed && !isHitApplied)
            {
                CancelAttackAnimation();

                statePayload.PlayerState = PlayerState.Balanced;
                statePayload.LastStateChangeTick = statePayload.Tick;
                return;
            }

            //Primary Attack Damage Tick
            if (isHitApplied == false && primaryAttack.HitApplicationTime <= attackCompletionPercentage)
            {
                ApplyHit(statePayload.Position, primaryAttack);
            }

            //End Primary attack
            if (primaryAttack.AttackDuration <= (statePayload.Tick - statePayload.LastStateChangeTick) * predictedPlayerTransform.ServerTickMs)
            {
                //Start Secondary attack
                if (inputPayload.AttackPressed)
                {
                    isHitApplied = false;
                    statePayload.PlayerState = PlayerState.Attacking_Secondary;
                    TriggerAttackAnimation(secondaryAttack.AnimationHash);
                }
                else
                    statePayload.PlayerState = PlayerState.Balanced;

                statePayload.LastStateChangeTick = statePayload.Tick;
                return;
            }

            if((primaryAttack.LungeStartTime <= attackCompletionPercentage && primaryAttack.LungeEndTime >= attackCompletionPercentage))
                characterController.Move(primaryAttack.LungeDistance * inputPayload.TickDuration * transform.forward / primaryAttack.LungeDuration);

            statePayload.Position = transform.position;
        }

        //During Secondary Attack
        if (statePayload.PlayerState.Equals(PlayerState.Attacking_Secondary))
        {
            float attackCompletionPercentage = (statePayload.Tick - statePayload.LastStateChangeTick) * predictedPlayerTransform.ServerTickMs / primaryAttack.AttackDuration;

            //Exit Secondary Attack
            if (!inputPayload.AttackPressed && !isHitApplied)
            {
                statePayload.PlayerState = PlayerState.Balanced;
                statePayload.LastStateChangeTick = statePayload.Tick;

                CancelAttackAnimation();

                return;
            }

            //Secondary Attack Damage Tick
            if (isHitApplied == false && secondaryAttack.HitApplicationTime <= (statePayload.Tick - statePayload.LastStateChangeTick) * predictedPlayerTransform.ServerTickMs / secondaryAttack.AttackDuration)
            {
                ApplyHit(statePayload.Position, secondaryAttack);
            }

            //End Secondary attack
            if (secondaryAttack.AttackDuration <= (statePayload.Tick - statePayload.LastStateChangeTick) * predictedPlayerTransform.ServerTickMs)
            {
                //Start Tertiary attack
                if (inputPayload.AttackPressed)
                {
                    isHitApplied = false;
                    statePayload.PlayerState = PlayerState.Attacking_Tertiary;

                    TriggerAttackAnimation(tertiaryAttack.AnimationHash);

                }
                else
                    statePayload.PlayerState = PlayerState.Balanced;

                statePayload.LastStateChangeTick = inputPayload.Tick;
                return;
            }

            if ((secondaryAttack.LungeStartTime <= attackCompletionPercentage && secondaryAttack.LungeEndTime >= attackCompletionPercentage))
                characterController.Move(secondaryAttack.LungeDistance * inputPayload.TickDuration * transform.forward / secondaryAttack.LungeDuration);

            statePayload.Position = transform.position;
        }

        //During Tertiary Attack
        if (statePayload.PlayerState.Equals(PlayerState.Attacking_Tertiary))
        {
            float attackCompletionPercentage = (statePayload.Tick - statePayload.LastStateChangeTick) * predictedPlayerTransform.ServerTickMs / primaryAttack.AttackDuration;

            //Exit Tertiary Attack
            if (!inputPayload.AttackPressed && !isHitApplied)
            {
                statePayload.PlayerState = PlayerState.Balanced;
                CancelAttackAnimation();

                return;
            }

            //Tertiary Attack Damage Tick
            if (isHitApplied == false && tertiaryAttack.HitApplicationTime <= (statePayload.Tick - statePayload.LastStateChangeTick) * predictedPlayerTransform.ServerTickMs / tertiaryAttack.AttackDuration)
            {
                ApplyHit(statePayload.Position, tertiaryAttack);
            }

            //End Tertiary attack
            if (tertiaryAttack.AttackDuration <= (statePayload.Tick - statePayload.LastStateChangeTick) * predictedPlayerTransform.ServerTickMs)
            {
                //Restart from Primary attack
                if (inputPayload.AttackPressed)
                {
                    statePayload.PlayerState = PlayerState.Attacking_Primary;

                    TriggerAttackAnimation(primaryAttack.AnimationHash);
                }
                else
                    statePayload.PlayerState = PlayerState.Balanced;

                statePayload.LastStateChangeTick = inputPayload.Tick;
                return;
            }

            if ((tertiaryAttack.LungeStartTime <= attackCompletionPercentage && tertiaryAttack.LungeEndTime >= attackCompletionPercentage))
                characterController.Move(tertiaryAttack.LungeDistance * inputPayload.TickDuration * transform.forward / tertiaryAttack.LungeDuration);

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
        
        if(isServer)
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

        if (isServer)
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
