using Mirror;
using UnityEngine;

public class PredictedPlayerBlock : DuelistControllerModule, IDuelistInputProcessor, IDuelistInputRecorder
{

    #region EDITOR EXPOSED FIELDS

    [Header("Block")]
    [SerializeField] BlockDefinition block;

    #endregion

    bool _isBlockButtonPressed;
    Animator animator;

    public bool IsBlockButtonPressed { set { _isBlockButtonPressed = value; } }

    public void RecordInput(ref InputPayload inputPayload)
    {
        inputPayload.BlockPressed = _isBlockButtonPressed;
    }

    public void ProcessInput(ref StatePayload statePayload, InputPayload inputPayload)
    {
        //Start Block
        if (inputPayload.BlockPressed && statePayload.CombatState.Equals(CombatState.Balanced))
        {
            statePayload.CombatState = CombatState.Blocking;
            statePayload.LastStateChangeTick = statePayload.Tick;

            TriggerBlockAnimation(block.AnimationHash);
        }

        //During Block
        if (statePayload.CombatState.Equals(CombatState.Blocking))
        {
            //End Block
            if (block.BlockDuration <= (statePayload.Tick - statePayload.LastStateChangeTick) * duelistCharacterController.ServerSendInterval)
            {
                statePayload.CombatState = CombatState.Balanced;
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
