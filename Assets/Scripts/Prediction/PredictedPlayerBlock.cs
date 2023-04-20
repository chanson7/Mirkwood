using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerEnergy))]
[RequireComponent(typeof(PlayerBalance))]
[RequireComponent(typeof(CharacterController))]
public class PredictedPlayerBlock : PredictedPlayerTickProcessor
{
    Animator animator;
    CharacterController characterController;
    PlayerEnergy playerEnergy;
    PlayerBalance playerBalance;
    [SerializeField] int blockEnergyCost;
    static int blockHash = Animator.StringToHash("Block");
    bool isBlocking = false;

    public override void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        playerEnergy = gameObject.GetComponent<PlayerEnergy>();
        playerBalance = gameObject.GetComponent<PlayerBalance>();
        characterController = gameObject.GetComponent<CharacterController>();

        base.Start();
    }

    void OnBlock(InputValue input)
    {
        if (predictedPlayerTransform.canPlayerAct && playerEnergy.GetEnergy() >= blockEnergyCost)
            isBlocking = true;
    }

    public override InputPayload GatherInput(InputPayload inputPayload)
    {
        if (isBlocking && inputPayload.ActiveAction > PlayerAnimationEvent.Block) //we aren't blocking but we should be
            inputPayload.ActiveAction = PlayerAnimationEvent.Block;

        else if (!isBlocking && inputPayload.ActiveAction == PlayerAnimationEvent.Block) //we are blocking but we shouldn't be
            inputPayload.ActiveAction = PlayerAnimationEvent.None;

        return inputPayload;
    }

    public override StatePayload ProcessTick(StatePayload statePayload, InputPayload inputPayload)
    {
        if (predictedPlayerTransform.canPlayerAct && inputPayload.ActiveAction == PlayerAnimationEvent.Block)
            StartBlock();

        return statePayload;
    }

    void StartBlock()
    {
        if (isServer && playerEnergy.SpendEnergy(blockEnergyCost)) //running on the server and the player has enough energy to block
        {
            animator.SetTrigger(blockHash);
            predictedPlayerTransform.canPlayerAct = false;
        }
        else //running on the client, energy check was predicted already in OnBlock().
        {
            animator.SetTrigger(blockHash);
            predictedPlayerTransform.canPlayerAct = false;
        }
    }

    void EndBlock()
    {
        isBlocking = false;
        predictedPlayerTransform.canPlayerAct = true;
    }
}
