using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class PredictedPlayerMeleeAttack : PredictedTransformModule, IPredictedInputRecorder, IPredictedInputProcessor
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
    [Tooltip("The time at which the hit should be applied, as a percentage of the attack duration")]
    float hitApplicationTime = 0.5f;

    [SerializeField]
    [Tooltip("Multiplier for the distance that the player is knocked back")]
    float knockbackMultiplier = 1f;

    [SerializeField]
    [Tooltip("The duration of the stun effect")]
    float stunDuration = 0.2f;

    [Header("")]
    [SerializeField]
    AreaOfEffect meleeAreaOfEffect;

    #endregion

    #region FIELDS

    bool isAttackButtonPressed;
    bool isHitApplied = true;
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

    public void ProcessInput(ref StatePayload statePayload, InputPayload inputPayload)
    {

        //Start First Attack
        if (inputPayload.AttackPressed && statePayload.PlayerState.Equals(PlayerState.Balanced))
        {
            statePayload.PlayerState = PlayerState.Attack1;
            statePayload.LastStateChangeTick = statePayload.Tick;

            isHitApplied = false;

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
                return;
            }

            //First Attack Damage Tick
            if (isHitApplied == false && hitApplicationTime <= (statePayload.Tick - statePayload.LastStateChangeTick) * predictedPlayerTransform.ServerTickMs / attackDuration)
            {
                ApplyHit(statePayload.Position);
            }

            //End First attack
            if (attackDuration <= (statePayload.Tick - statePayload.LastStateChangeTick) * predictedPlayerTransform.ServerTickMs)
            {
                //Start second attack
                if (inputPayload.AttackPressed)
                {
                    isHitApplied = false;
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
                return;
            }

            //Second Attack Damage Tick
            if (isHitApplied == false && hitApplicationTime <= (statePayload.Tick - statePayload.LastStateChangeTick) * predictedPlayerTransform.ServerTickMs / attackDuration)
            {
                ApplyHit(statePayload.Position);
            }

            //End Second attack
            if (attackDuration <= (statePayload.Tick - statePayload.LastStateChangeTick) * predictedPlayerTransform.ServerTickMs)
            {
                //Start Third attack
                if (inputPayload.AttackPressed)
                {
                    isHitApplied = false;
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

                return;
            }

            //Final Attack Damage Tick
            if (isHitApplied == false && hitApplicationTime <= (statePayload.Tick - statePayload.LastStateChangeTick) * predictedPlayerTransform.ServerTickMs / attackDuration)
            {
                ApplyHit(statePayload.Position);
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

    void ApplyHit(Vector3 myPosition)
    {
        foreach (Transform other in meleeAreaOfEffect.Collisions)
        {
            if (other.GetComponent<PredictedPlayerDisable>() != null)
            {
                if(isServer)
                    other.GetComponent<PredictedPlayerDisable>().ServerTriggerInterrupt((other.transform.position - myPosition).normalized * knockbackMultiplier, stunDuration);
            }
        }

        isHitApplied = true;
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
