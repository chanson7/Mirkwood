using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class PredictedPlayerBlock : PredictionModule, IPredictedInputProcessor, IPredictedInputRecorder
{

    #region EDITOR EXPOSED FIELDS

    [Header("Block")]
    [SerializeField] BlockDefinition block;

    #endregion

    bool isBlockButtonPressed;
    Animator animator;

    #region INPUT

    void OnBlock(InputValue input)
    {
        isBlockButtonPressed = input.isPressed;
    }

    public void RecordInput(ref InputPayload inputPayload)
    {
        inputPayload.BlockPressed = isBlockButtonPressed;
    }

    #endregion

    public void ProcessInput(ref StatePayload statePayload, InputPayload inputPayload)
    {
        //Start Block
        if (inputPayload.BlockPressed && statePayload.PlayerState.Equals(PlayerState.Balanced))
        {
            statePayload.PlayerState = PlayerState.Blocking;
            statePayload.LastStateChangeTick = statePayload.Tick;

            TriggerBlockAnimation(block.AnimationHash);
        }

        //During Block
        if (statePayload.PlayerState.Equals(PlayerState.Blocking))
        {
            //End Block
            if (block.BlockDuration <= (statePayload.Tick - statePayload.LastStateChangeTick) * predictedCharacterController.ServerTickMs)
            {
                statePayload.PlayerState = PlayerState.Balanced;
                statePayload.LastStateChangeTick = statePayload.Tick;
            }
        }

        statePayload.Position = transform.position;
    }

    void TriggerBlockAnimation(int blockHash)
    {
        if (isLocalPlayer)
            animator.SetTrigger(blockHash);
        
        if (isServer)
            RpcTriggerAttackAnimation(blockHash);

        [ClientRpc(includeOwner = false)]
        void RpcTriggerAttackAnimation(int blockHash)
        {
            animator.SetTrigger(blockHash);
        }
    }

    public void Awake()
    {
        animator = GetComponent<Animator>();
    }

}
