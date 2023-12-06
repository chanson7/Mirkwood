using Mirror;
using UnityEngine;

public class DuelistDodge : DuelistControllerModule, IDuelistInputProcessor, IDuelistInputRecorder
{

    #region EDITOR EXPOSED FIELDS

    [Header("Dodge")]
    [SerializeField] DodgeDefinition dodge;

    #endregion

    bool _isDodgeButtonPressed;
    Animator animator;
    CharacterController characterController;

    public bool IsDodgeButtonPressed { set { _isDodgeButtonPressed = value; } }

    static readonly int dodgeForwardHash = Animator.StringToHash("DodgeForward");
    static readonly int dodgeRightHash = Animator.StringToHash("DodgeRight");

    public void RecordInput(ref InputPayload inputPayload)
    {
        inputPayload.DodgePressed = _isDodgeButtonPressed;
    }

    public void ProcessInput(ref StatePayload statePayload, InputPayload inputPayload)
    {
        //Start Dodge
        if (inputPayload.DodgePressed && statePayload.CombatState.Equals(CombatState.Balanced))
        {
            statePayload.CombatState = CombatState.Dodging;
            statePayload.LastStateChangeTick = statePayload.Tick;

            TriggerDodgeAnimation(dodge.AnimationHash, inputPayload.MoveDirection);
        }

        //During Dodge
        if (statePayload.CombatState.Equals(CombatState.Dodging))
        {
            //End Dodge
            if (dodge.DodgeDuration <= (statePayload.Tick - statePayload.LastStateChangeTick) * duelistCharacterController.ServerSendInterval)
            {
                statePayload.CombatState = CombatState.Balanced;
                statePayload.LastStateChangeTick = statePayload.Tick;
            }
            else
            {
                characterController.Move(dodge.DodgeDistance * inputPayload.TickDuration * statePayload.Velocity.normalized / dodge.DodgeDuration);
            }
        }

        statePayload.Position = transform.position;
    }

    void TriggerDodgeAnimation(int dodgeHash, Vector2 dodgeDirection)
    {
        if (isLocalPlayer)
        {
            animator.SetFloat(dodgeForwardHash, dodgeDirection.normalized.y);
            animator.SetFloat(dodgeRightHash, dodgeDirection.normalized.x);
            animator.SetTrigger(dodgeHash);
        }
        if (isServer)
            RpcTriggerAttackAnimation(dodgeHash, dodgeDirection);

        [ClientRpc(includeOwner = false)]
        void RpcTriggerAttackAnimation(int dodgeHash, Vector2 dodgeDirection)
        {
            animator.SetFloat(dodgeForwardHash, dodgeDirection.normalized.y);
            animator.SetFloat(dodgeRightHash, dodgeDirection.normalized.x);
            animator.SetTrigger(dodgeHash);
        }
    }

    public void Awake()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
    }

}
