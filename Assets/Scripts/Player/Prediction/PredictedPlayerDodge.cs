using Mirror;
using UnityEngine;

public class PredictedPlayerDodge : PredictionModule, IPredictedInputProcessor, IPredictedInputRecorder
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
        if (inputPayload.DodgePressed && statePayload.PlayerState.Equals(PlayerState.Balanced))
        {
            statePayload.PlayerState = PlayerState.Dodging;
            statePayload.LastStateChangeTick = statePayload.Tick;

            TriggerDodgeAnimation(dodge.AnimationHash, inputPayload.MoveDirection);
        }

        //During Dodge
        if (statePayload.PlayerState.Equals(PlayerState.Dodging))
        {
            //End Dodge
            if (dodge.DodgeDuration <= (statePayload.Tick - statePayload.LastStateChangeTick) * predictedCharacterController.ServerSendInterval)
            {
                statePayload.PlayerState = PlayerState.Balanced;
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
