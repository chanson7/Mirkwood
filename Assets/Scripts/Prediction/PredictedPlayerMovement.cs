using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class PredictedPlayerMovement : PredictedTransformModule, IPredictedInputRecorder, IPredictedStateProcessor
{
    #region EDITOR EXPOSED FIELDS

    [Header("Movement Settings")]

    [Tooltip("The speed multiplier for when the player moves forward")]
    [SerializeField] float _runSpeed;

    [Tooltip("The speed multiplier for when the player strafes sideways")]
    [SerializeField] float _strafeSpeed;

    [Tooltip("The speed multiplier for when the player moves backwards")]
    [SerializeField] float _backpedalSpeed;

    #endregion

    #region FIELDS

    Vector2 movementInput = Vector2.zero;
    Animator animator;
    CharacterController characterController;

    static readonly int forwardHash = Animator.StringToHash("Forward");
    static readonly int rightHash = Animator.StringToHash("Right");

    #endregion

    #region INPUT

    void OnMove(InputValue input)
    {
        movementInput.x = input.Get<Vector2>().x;
        movementInput.y = input.Get<Vector2>().y;
    }

    #endregion

    #region METHODS

    public void RecordInput(ref InputPayload inputPayload)
    {
        inputPayload.MoveDirection = movementInput;
    }

    public void ProcessTick(ref StatePayload statePayload, InputPayload inputPayload)
    {
        Vector3 movementVelocity = Vector3.zero;

        if (statePayload.PlayerState.Equals(PlayerState.Balanced))
        {
            Vector3 previousPosition = statePayload.Position;
            Vector3 desiredMovement = (_strafeSpeed * inputPayload.MoveDirection.x * transform.right +
                transform.forward * Mathf.Clamp(inputPayload.MoveDirection.y * _runSpeed, -_backpedalSpeed, _runSpeed)) * inputPayload.TickDuration;

            characterController.Move(desiredMovement);

            movementVelocity = (transform.position - previousPosition) / inputPayload.TickDuration;
            statePayload.Position = transform.position;
        }

        if (isLocalPlayer)
            AnimateMovement(movementVelocity);
        else if (isServerOnly)
            RpcAnimateMovement(movementVelocity);

    }

    void AnimateMovement(Vector3 currentVelocity)
    {
        animator.SetFloat(forwardHash, transform.InverseTransformDirection(currentVelocity).z);
        animator.SetFloat(rightHash, transform.InverseTransformDirection(currentVelocity).x);
    }

    [ClientRpc(includeOwner = false)]
    void RpcAnimateMovement(Vector3 currentVelocity)
    {
        animator.SetFloat(forwardHash, transform.InverseTransformDirection(currentVelocity).z);
        animator.SetFloat(rightHash, transform.InverseTransformDirection(currentVelocity).x);
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
