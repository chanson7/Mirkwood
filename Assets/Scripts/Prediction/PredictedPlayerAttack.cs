using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class PredictedPlayerAttack : PredictedTransformModule, IPredictedInputRecorder, IPredictedStateProcessor
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
    float damageApplicationPoint = 0.5f;

    [Header("Melee Colliders")]
    [SerializeField] MeleeCollider forwardMeleeCollider;
    [SerializeField] MeleeCollider aoeMeleeCollider;

    #endregion

    #region FIELDS

    bool attackButtonPressedThisTick;
    bool damageApplied = false;
    CharacterController characterController;
    Animator animator;

    static readonly int attackHash = Animator.StringToHash("Attack");

    #endregion

    #region INPUT

    void OnAttack(InputValue input)
    {
        attackButtonPressedThisTick = input.isPressed;
    }

    #endregion

    public void RecordInput(ref InputPayload inputPayload)
    {
        inputPayload.AttackPressed = attackButtonPressedThisTick;
        attackButtonPressedThisTick = false;
    }

    public void ProcessTick(ref StatePayload statePayload, InputPayload inputPayload)
    {

        //Begin Attack
        if (inputPayload.AttackPressed && statePayload.PlayerState.Equals(PlayerState.Balanced))
        {
            statePayload.PlayerState = PlayerState.Attacking;
            statePayload.LastStateChangeTick = inputPayload.Tick;

            damageApplied = false;

            if (isLocalPlayer)
                TriggerAttackAnimation();
            else if (isServer)
                RpcTriggerAttackAnimation();
        }

        //During Attack
        if (statePayload.PlayerState.Equals(PlayerState.Attacking))
        {

            //Damage Tick
            if(damageApplied == false && damageApplicationPoint <= (statePayload.Tick - statePayload.LastStateChangeTick) * predictedPlayerTransform.ServerTickMs / attackDuration)
            {
                if (isServer)
                    ServerApplyMeleeDamage();

                damageApplied = true;
            }

            //End attack
            if(attackDuration <= (statePayload.Tick - statePayload.LastStateChangeTick) * predictedPlayerTransform.ServerTickMs)
            {
                statePayload.PlayerState = PlayerState.Balanced;
                statePayload.LastStateChangeTick = inputPayload.Tick;
                return;
            }

            characterController.Move(lungeDistance * inputPayload.TickDuration * transform.forward / attackDuration);

            statePayload.Position = transform.position;
        }
    }

    [Server]
    void ServerApplyMeleeDamage()
    {
        foreach (Collider collider in forwardMeleeCollider.DamageableColliders)
        {
            if (collider.enabled) {
                Debug.Log($"Dealing damage to {collider.gameObject.name}");
            }
        }
    }

    void TriggerAttackAnimation()
    {
        animator.SetTrigger(attackHash);
    }

    [ClientRpc(includeOwner = false)]
    void RpcTriggerAttackAnimation()
    {
        animator.SetTrigger(attackHash);
    }

    #region MONOBEHAVIOUR

    public override void Start()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();

        base.Start();
    }

    #endregion

}
