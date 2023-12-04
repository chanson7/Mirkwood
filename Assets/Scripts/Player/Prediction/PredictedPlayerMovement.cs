using UnityEngine;
using Mirror;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class PredictedPlayerMovement : PredictionModule, IPredictedInputRecorder, IPredictedInputProcessor
{
    #region EDITOR EXPOSED FIELDS

    [Header("Movement Settings")]
    [Tooltip("The speed multiplier for when the player moves forward")]
    [SerializeField] float runSpeed;

    [Tooltip("The speed multiplier for when the player strafes sideways")]
    [SerializeField] float strafeSpeed;

    [Tooltip("The speed multiplier for when the player moves backwards")]
    [SerializeField] float backpedalSpeed;

    [Tooltip("Determines how quickly a player's velocity changes. \n0 = player can't move\n1 = player instantly changes velocity")]
    [Range(0f, 1f)]
    [SerializeField] float acceleration;

    #endregion

    #region FIELDS

    Vector2 _movementInput = Vector2.zero;
    Animator animator;
    CharacterController characterController;

    static readonly int moveForwardHash = Animator.StringToHash("MoveForward");
    static readonly int moveRightHash = Animator.StringToHash("MoveRight");

    #endregion

    #region PROPERTIES

    public Vector2 MovementInput { set { _movementInput = value; } }

    #endregion

    #region METHODS

    public void RecordInput(ref InputPayload inputPayload)
    {
        inputPayload.MoveDirection = _movementInput;
    }

    public void ProcessInput(ref StatePayload statePayload, InputPayload inputPayload)
    {

        if (statePayload.PlayerState.Equals(PlayerState.Balanced))
        {
            Vector3 previousPosition = statePayload.Position;
            Vector3 previousVelocity = statePayload.Velocity * inputPayload.TickDuration;

            Vector3 desiredMovementVelocity = inputPayload.TickDuration * 
                                              (strafeSpeed * inputPayload.MoveDirection.x * transform.right + 
                                              transform.forward * Mathf.Clamp(inputPayload.MoveDirection.y * runSpeed, -backpedalSpeed, runSpeed));

            characterController.Move(Vector3.Lerp(previousVelocity, desiredMovementVelocity, acceleration));

            Vector3 movementVelocity = (transform.position - previousPosition) / inputPayload.TickDuration;

            statePayload.Position = transform.position;
        
            AnimateMovement(movementVelocity);
        }

    }

    void AnimateMovement(Vector3 currentVelocity)
    {
        if (isLocalPlayer)
        {
            animator.SetFloat(moveForwardHash, transform.InverseTransformDirection(currentVelocity).z);
            animator.SetFloat(moveRightHash, transform.InverseTransformDirection(currentVelocity).x);
        }
        
        if (isServer)
            RpcAnimateMovement(currentVelocity);

        [ClientRpc(includeOwner = false)]
        void RpcAnimateMovement(Vector3 currentVelocity)
        {
            animator.SetFloat(moveForwardHash, transform.InverseTransformDirection(currentVelocity).z);
            animator.SetFloat(moveRightHash, transform.InverseTransformDirection(currentVelocity).x);
        }

    }

    #endregion

    #region MONOBEHAVIOUR

    public void Awake()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
    }

    #endregion

}
