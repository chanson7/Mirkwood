using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerEnergy))]
[RequireComponent(typeof(PlayerBalance))]
[RequireComponent(typeof(CharacterController))]
public class PredictedPlayerAttack : PredictedPlayerInputProcessor
{
    [SerializeField] MeleeCollision meleeCollision;
    [SerializeField] int attackEnergyCost;
    [SerializeField] float attackMovementSpeed;
    [Tooltip("The amount of balance lost upon a missed attack.")]
    [SerializeField] int missPenalty;
    Animator animator;
    PlayerEnergy playerEnergy;
    PlayerBalance playerBalance;
    CharacterController characterController;
    static int attackHash = Animator.StringToHash("Attack");
    bool isAttacking = false;

    public override void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        playerEnergy = gameObject.GetComponent<PlayerEnergy>();
        playerBalance = gameObject.GetComponent<PlayerBalance>();
        characterController = gameObject.GetComponent<CharacterController>();

        base.Start();
    }

    void OnAttack(InputValue input)
    {
        if (predictedPlayerTransform.canPlayerAct && playerEnergy.GetEnergy() >= attackEnergyCost)
            isAttacking = true;
    }

    public override InputPayload GatherInput(InputPayload inputPayload)
    {
        if (isAttacking && inputPayload.ActiveAction > PlayerAnimationEvent.Attack) //we aren't attacking but we should be
            inputPayload.ActiveAction = PlayerAnimationEvent.Attack;

        else if (!isAttacking && inputPayload.ActiveAction == PlayerAnimationEvent.Attack) //we are attacking but we shouldn't be
            inputPayload.ActiveAction = PlayerAnimationEvent.None;

        return inputPayload;
    }

    //this is where "root motion" for attack animations happens
    public override StatePayload ProcessTick(StatePayload statePayload, InputPayload inputPayload)
    {
        if (predictedPlayerTransform.canPlayerAct && inputPayload.ActiveAction == PlayerAnimationEvent.Attack)
            StartAttack();

        if (inputPayload.ActiveAction == PlayerAnimationEvent.Attack)
        {
            characterController.Move(transform.forward * (1f / MirkwoodNetworkManager.singleton.serverTickRate) * attackMovementSpeed);
        }

        statePayload.Position = transform.position;
        statePayload.CurrentVelocity = characterController.velocity;

        return statePayload;
    }

    public override void OnInterrupt()
    {
        throw new System.NotImplementedException();
    }

    void StartAttack()
    {
        if (isServer && playerEnergy.SpendEnergy(attackEnergyCost)) //on the server and the player has enough energy to attack
        {
            animator.SetTrigger(attackHash);
            predictedPlayerTransform.canPlayerAct = false;
        }
        else //running on the local client
        {
            animator.SetTrigger(attackHash);
            predictedPlayerTransform.canPlayerAct = false;
        }
    }

    void EndAttack()
    {
        isAttacking = false;
        predictedPlayerTransform.canPlayerAct = true;
    }

    void HitMeleeColliders(float angularDifferenceFromForwardVector)
    {
        int hitCount = 0;

        if (isServer)
        {
            foreach (Collider collider in meleeCollision.damageableColliders)
            {
                Vector3 directionToOtherTransform = collider.transform.position - this.transform.position;
                float angle = Vector3.Angle(transform.forward, directionToOtherTransform);

                if (angle < angularDifferenceFromForwardVector)
                {
                    hitCount++;
                    Debug.Log($"..Hit {collider.name}");
                }
            }

            if (hitCount < 1)
            {
                Debug.Log("..Attack missed");
                playerBalance.LoseBalance(missPenalty);
            }

        }
    }

}
