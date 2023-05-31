using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerEnergy))]
[RequireComponent(typeof(PlayerBalance))]
[RequireComponent(typeof(CharacterController))]
public class PredictedPlayerDodge : PredictedPlayerInputProcessor
{
    Animator animator;
    CharacterController characterController;
    PlayerEnergy playerEnergy;
    PlayerBalance playerBalance;
    [SerializeField] int dodgeEnergyCost;
    [SerializeField] float dodgeMovementSpeed;
    [SerializeField] int dodgeBalanceLoss;
    static int dodgeHash = Animator.StringToHash("Dodge");
    bool isDodging = false;
    Vector3 dodgeDirection = Vector3.zero;

    public override void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        playerEnergy = gameObject.GetComponent<PlayerEnergy>();
        playerBalance = gameObject.GetComponent<PlayerBalance>();
        characterController = gameObject.GetComponent<CharacterController>();

        base.Start();
    }

    void OnDodge(InputValue input)
    {
        if (predictedPlayerTransform.canPlayerAct && playerEnergy.GetEnergy() >= dodgeEnergyCost)
            isDodging = true;
    }

    public override InputPayload GatherInput(InputPayload inputPayload)
    {
        if (isDodging && inputPayload.ActiveAction > PlayerAnimationEvent.Dodge) //we aren't dodging but we should be
            inputPayload.ActiveAction = PlayerAnimationEvent.Dodge;

        else if (!isDodging && inputPayload.ActiveAction == PlayerAnimationEvent.Dodge) //we are dodging but we should stop (maybe we were interrupted somehow)
            inputPayload.ActiveAction = PlayerAnimationEvent.None;

        return inputPayload;
    }

    public override StatePayload ProcessTick(StatePayload statePayload, InputPayload inputPayload)
    {

        if (predictedPlayerTransform.canPlayerAct && inputPayload.ActiveAction == PlayerAnimationEvent.Dodge)
        {
            dodgeDirection = inputPayload.MoveDirection.normalized;
            StartDodge();
        }

        if (inputPayload.ActiveAction == PlayerAnimationEvent.Dodge)
        {
            characterController.Move(dodgeDirection * (1f / MirkwoodNetworkManager.singleton.serverTickRate) * dodgeMovementSpeed);
        }

        statePayload.Position = transform.position;
        statePayload.CurrentVelocity = characterController.velocity;

        return statePayload;
    }

    public override void OnInterrupt()
    {
        throw new System.NotImplementedException();
    }

    void StartDodge()
    {
        if (isServer && playerEnergy.SpendEnergy(dodgeEnergyCost)) //running on the server and the player has enough energy to dodge
        {
            playerBalance.LoseBalance(dodgeBalanceLoss);
            animator.SetTrigger(dodgeHash);
            predictedPlayerTransform.canPlayerAct = false;
        }
        else
        {
            animator.SetTrigger(dodgeHash);
            predictedPlayerTransform.canPlayerAct = false;
        }
    }

    public void EndDodge()
    {
        dodgeDirection = Vector3.zero;
        isDodging = false;
        predictedPlayerTransform.canPlayerAct = true;
    }
}
