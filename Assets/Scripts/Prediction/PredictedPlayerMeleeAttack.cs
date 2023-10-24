using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class PredictedPlayerMeleeAttack : PredictedTransformModule, IPredictedInputRecorder, IPredictedStateProcessor
{

    #region EDITOR EXPOSED FIELDS

    [Header("Attack Settings")]
    [SerializeField]
    [Tooltip("The length of time an attack lasts (in seconds)")]
    float attackDuration = 1f;

    [SerializeField]
    [Tooltip("The distance a player should move forwards during the attack animation")]
    float lungeDistance = 1f;

    [SerializeField]
    [Range(0f,1f)]
    [Tooltip("The tick at which damage should be applied, as a percentage of the attack duration")]
    float damageApplicationTime = 0.5f;

    [Header("")]
    [SerializeField]
    AreaOfEffect meleeAreaOfEffect;

    #endregion

    #region FIELDS

    bool isAttackButtonPressed;
    bool isDamageApplied = true;
    CharacterController characterController;
    Animator animator;

    static readonly int attack1Hash = Animator.StringToHash("Attack1");
    static readonly int attack2Hash = Animator.StringToHash("Attack2");
    static readonly int attack3Hash = Animator.StringToHash("Attack3");

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

    public void ProcessTick(ref StatePayload statePayload, InputPayload inputPayload)
    {

        //Start First Attack
        if (inputPayload.AttackPressed && statePayload.PlayerState.Equals(PlayerState.Balanced))
        {
            statePayload.PlayerState = PlayerState.Attack1;
            statePayload.LastStateChangeTick = statePayload.Tick;

            isDamageApplied = false;

            if (isLocalPlayer)
                TriggerAttackAnimation(attack1Hash);
            if (isServer)
                RpcTriggerAttackAnimation(attack1Hash);
        }

        //During First Attack
        if (statePayload.PlayerState.Equals(PlayerState.Attack1))
        {
            //Exit First Attack
            if (!inputPayload.AttackPressed)
            {
                statePayload.PlayerState = PlayerState.Balanced;
                statePayload.LastStateChangeTick = statePayload.Tick;
                //if (isLocalPlayer)
                //    TriggerAttackAnimation();
                //if (isServer)
                //    RpcTriggerAttackAnimation();
                return;
            }

            //First Attack Damage Tick
            if (isDamageApplied == false && damageApplicationTime <= (statePayload.Tick - statePayload.LastStateChangeTick) * predictedPlayerTransform.ServerTickMs / attackDuration)
            {
                ApplyKnockback();

                isDamageApplied = true;
            }

            //End First attack
            if (attackDuration <= (statePayload.Tick - statePayload.LastStateChangeTick) * predictedPlayerTransform.ServerTickMs)
            {
                //Start second attack
                if (inputPayload.AttackPressed)
                {
                    isDamageApplied = false;
                    statePayload.PlayerState = PlayerState.Attack2;

                    if (isLocalPlayer)
                        TriggerAttackAnimation(attack2Hash);
                    if (isServer)
                        RpcTriggerAttackAnimation(attack2Hash);
                }
                else
                    statePayload.PlayerState = PlayerState.Balanced;

                statePayload.LastStateChangeTick = statePayload.Tick;
                return;
            }

            characterController.Move(lungeDistance * inputPayload.TickDuration * transform.forward / attackDuration);

            statePayload.Position = transform.position;
        }

        //During Second Attack
        if (statePayload.PlayerState.Equals(PlayerState.Attack2))
        {
            //Exit Second Attack
            if (!inputPayload.AttackPressed)
            {
                statePayload.PlayerState = PlayerState.Balanced;
                statePayload.LastStateChangeTick = statePayload.Tick;
                //if (isLocalPlayer)
                //    TriggerAttackAnimation();
                //if (isServer)
                //    RpcTriggerAttackAnimation();
                return;
            }

            //Second Attack Damage Tick
            if (isDamageApplied == false && damageApplicationTime <= (statePayload.Tick - statePayload.LastStateChangeTick) * predictedPlayerTransform.ServerTickMs / attackDuration)
            {
                ApplyKnockback();

                isDamageApplied = true;
            }

            //End Second attack
            if (attackDuration <= (statePayload.Tick - statePayload.LastStateChangeTick) * predictedPlayerTransform.ServerTickMs)
            {
                //Start Third attack
                if (inputPayload.AttackPressed)
                {
                    isDamageApplied = false;
                    statePayload.PlayerState = PlayerState.Attack3;

                    if (isLocalPlayer)
                        TriggerAttackAnimation(attack3Hash);
                    if (isServer)
                        RpcTriggerAttackAnimation(attack3Hash);
                }
                else
                    statePayload.PlayerState = PlayerState.Balanced;

                statePayload.LastStateChangeTick = inputPayload.Tick;
                return;
            }

            characterController.Move(lungeDistance * inputPayload.TickDuration * transform.forward / attackDuration);

            statePayload.Position = transform.position;
        }

        //During Final Attack
        if (statePayload.PlayerState.Equals(PlayerState.Attack3))
        {

            //Exit Final Attack
            if (!inputPayload.AttackPressed)
            {
                statePayload.PlayerState = PlayerState.Balanced;

                //if (isLocalPlayer)
                //    TriggerAttackAnimation();
                //if (isServer)
                //    RpcTriggerAttackAnimation();
                return;
            }

            //Final Attack Damage Tick
            if (isDamageApplied == false && damageApplicationTime <= (statePayload.Tick - statePayload.LastStateChangeTick) * predictedPlayerTransform.ServerTickMs / attackDuration)
            {
                ApplyKnockback();

                isDamageApplied = true;
            }

            //End Third attack
            if (attackDuration <= (statePayload.Tick - statePayload.LastStateChangeTick) * predictedPlayerTransform.ServerTickMs)
            {
                //Start First attack
                if (inputPayload.AttackPressed)
                {
                    statePayload.PlayerState = PlayerState.Attack1;

                    if (isLocalPlayer)
                        TriggerAttackAnimation(attack1Hash);
                    if (isServer)
                        RpcTriggerAttackAnimation(attack1Hash);
                }
                else
                    statePayload.PlayerState = PlayerState.Balanced;

                statePayload.LastStateChangeTick = inputPayload.Tick;
                return;
            }

            characterController.Move(lungeDistance * inputPayload.TickDuration * transform.forward / attackDuration);

            statePayload.Position = transform.position;
        }
    }

    void ApplyKnockback()
    {
        foreach (Transform other in meleeAreaOfEffect.Collisions)
        {
            //math here
            if(other.GetComponent<PredictedPlayerHit>() != null) {
                other.GetComponent<PredictedPlayerHit>().HitVector = Vector3.forward;
            }
        }
    }

    void TriggerAttackAnimation(int attackHash)
    {
        animator.SetTrigger(attackHash);
    }

    [ClientRpc(includeOwner = false)]
    void RpcTriggerAttackAnimation(int attackHash)
    {
        animator.SetTrigger(attackHash);
    }

    #region MONOBEHAVIOUR

    public void Awake()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
    }

    #endregion

}
