using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerEnergy))]
[RequireComponent(typeof(CharacterController))]
public class PredictedPlayerAttack : PredictedPlayerTickProcessor
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
    bool canAttack = true;

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
        if (canAttack && playerEnergy.GetEnergy() >= attackEnergyCost)
            isAttacking = true;
    }

    public override InputPayload GatherInput(InputPayload inputPayload)
    {
        if (isAttacking && inputPayload.ActiveAnimationPriority > AnimationPriority.Attack) //we aren't attacking but we should be
            inputPayload.ActiveAnimationPriority = AnimationPriority.Attack;

        else if (!isAttacking && inputPayload.ActiveAnimationPriority == AnimationPriority.Attack) //we are attacking but we shouldn't be
            inputPayload.ActiveAnimationPriority = AnimationPriority.None;

        return inputPayload;
    }

    //this is where "root motion" for attack animations happens
    public override StatePayload ProcessTick(StatePayload statePayload, InputPayload inputPayload)
    {
        if (canAttack && inputPayload.ActiveAnimationPriority == AnimationPriority.Attack)
            StartAttack();

        if (inputPayload.ActiveAnimationPriority == AnimationPriority.Attack)
        {
            characterController.Move(transform.forward * (1f / MirkwoodNetworkManager.singleton.serverTickRate) * attackMovementSpeed);
        }

        statePayload.Position = transform.position;
        statePayload.CurrentVelocity = characterController.velocity;

        return statePayload;
    }

    void StartAttack()
    {
        if (isServer && playerEnergy.SpendEnergy(attackEnergyCost)) //running on the server and the player has enough energy to attack
        {
            animator.SetTrigger(attackHash);
            canAttack = false;
        }
        else //running on the local client
        {
            animator.SetTrigger(attackHash);
            canAttack = false;
        }

    }

    void EndAttack()
    {
        isAttacking = false;
        canAttack = true;
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
