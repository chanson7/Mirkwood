using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerEnergy))]
[RequireComponent(typeof(CharacterController))]
public class PredictedPlayerAttack : PredictedPlayerTickProcessor
{
    [SerializeField] MeleeCollision meleeCollision;
    [SerializeField] uint attackEnergyCost;
    [SerializeField] float attackMovementSpeed;
    CharacterController characterController;
    Animator animator;
    PlayerEnergy playerEnergy;
    static int attackHash = Animator.StringToHash("Attack");
    bool isAttacking = false;
    bool canAttack = true;

    public override void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        characterController = gameObject.GetComponent<CharacterController>();
        playerEnergy = gameObject.GetComponent<PlayerEnergy>();

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
        animator.SetTrigger(attackHash);
        canAttack = false;
    }

    void EndAttack()
    {
        isAttacking = false;
        canAttack = true;
    }

    void DealDamage(float angularDifferenceFromForwardVector)
    {
        if (isServer)
        {
            foreach (Collider collider in meleeCollision.damageableColliders)
            {
                Vector3 directionToOtherTransform = collider.transform.position - this.transform.position;
                float angle = Vector3.Angle(transform.forward, directionToOtherTransform);

                if (angle < angularDifferenceFromForwardVector)
                {
                    Debug.Log($"..Deal damage to {collider.name} :O");
                }
            }
        }
    }

}
