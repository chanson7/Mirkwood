using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerEnergy))]
[RequireComponent(typeof(CharacterController))]
public class PredictedPlayerDodge : PredictedPlayerTickProcessor
{
    Animator animator;
    CharacterController characterController;
    PlayerEnergy playerEnergy;
    [SerializeField] uint dodgeEnergyCost;
    [SerializeField] float dodgeMovementSpeed;
    static int dodgeHash = Animator.StringToHash("Dodge");
    bool isDodging = false;
    bool canDodge = true;

    public override void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        characterController = gameObject.GetComponent<CharacterController>();
        playerEnergy = gameObject.GetComponent<PlayerEnergy>();

        base.Start();
    }

    void OnDodge(InputValue input)
    {
        if (canDodge && playerEnergy.GetEnergy() >= dodgeEnergyCost)
            isDodging = true;
    }

    public override InputPayload GatherInput(InputPayload inputPayload)
    {
        if (isDodging && inputPayload.ActiveAnimationPriority > AnimationPriority.Dodge) //we aren't dodging but we should be
            inputPayload.ActiveAnimationPriority = AnimationPriority.Dodge;

        else if (!isDodging && inputPayload.ActiveAnimationPriority == AnimationPriority.Dodge) //we are dodging but we shouldn't be
            inputPayload.ActiveAnimationPriority = AnimationPriority.None;

        return inputPayload;
    }

    public override StatePayload ProcessTick(StatePayload statePayload, InputPayload inputPayload)
    {

        if (canDodge && inputPayload.ActiveAnimationPriority == AnimationPriority.Dodge)
            StartDodge();

        if (inputPayload.ActiveAnimationPriority == AnimationPriority.Dodge)
        {
            characterController.Move(inputPayload.MoveDirection * (1f / MirkwoodNetworkManager.singleton.serverTickRate) * dodgeMovementSpeed);
        }

        statePayload.Position = transform.position;
        statePayload.CurrentVelocity = characterController.velocity;

        return statePayload;
    }

    void StartDodge()
    {
        animator.SetTrigger(dodgeHash);
        canDodge = false;
    }

    void EndDodge()
    {
        isDodging = false;
        canDodge = true;
    }
}
