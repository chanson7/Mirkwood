using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class PredictedPlayerMovement : PredictedTransformModule, IPredictedInputRecorder, IPredictedStateProcessor
{
    #region EDITOR EXPOSED FIELDS

    [SerializeField] float _runSpeed;
    [SerializeField] float _strafeSpeed;
    [SerializeField] float _backpedalSpeed;

    #endregion

    #region FIELDS

    Vector2 movementInput = Vector2.zero;
    Animator animator;
    CharacterController characterController;

    static readonly int forwardHash = Animator.StringToHash("Forward");
    static readonly int rightHash = Animator.StringToHash("Right");

    #endregion

    #region METHODS

    void OnMove(InputValue input)
    {
        movementInput.x = input.Get<Vector2>().x;
        movementInput.y = input.Get<Vector2>().y;
    }

    public void RecordInput(ref InputPayload inputPayload)
    {
        inputPayload.MoveDirection = movementInput;
    }

    public void ProcessTick(ref StatePayload statePayload, InputPayload inputPayload)
    {
        
        Vector3 previousPosition = statePayload.Position;
        Vector3 desiredMovement = (_strafeSpeed * inputPayload.MoveDirection.x * transform.right +
            transform.forward * Mathf.Clamp(inputPayload.MoveDirection.y * _runSpeed, -_backpedalSpeed, _runSpeed)) * inputPayload.TickDuration;

        characterController.Move(desiredMovement);

        Vector3 velocity = (transform.position - previousPosition) / inputPayload.TickDuration;

        if (isLocalPlayer)
            AnimateMovement(velocity);
        else if (isServer)
            RpcAnimateMovement(velocity);

        statePayload.Position = transform.position;
        statePayload.Velocity = velocity;
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

    public override void Start()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();

        base.Start();
    }

    #endregion

}
