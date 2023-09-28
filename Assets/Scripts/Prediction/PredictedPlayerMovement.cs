using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;
using Unity.VisualScripting;

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

    Vector3 _movementInput = Vector3.zero;
    Animator _animator;
    CharacterController _characterController;

    static readonly int _forwardHash = Animator.StringToHash("Forward");
    static readonly int _rightHash = Animator.StringToHash("Right");

    #endregion

    #region METHODS

    void OnMove(InputValue input)
    {
        _movementInput.x = input.Get<Vector2>().x;
        _movementInput.z = input.Get<Vector2>().y;
    }

    public void RecordInput(ref InputPayload inputPayload)
    {
        inputPayload.MoveDirection = _movementInput;
    }

    public void ProcessTick(ref StatePayload statePayload, InputPayload inputPayload)
    {
        
        Vector3 previousPosition = statePayload.Position;
        Vector3 desiredMovement = (_strafeSpeed * inputPayload.MoveDirection.x * transform.right +
            transform.forward * Mathf.Clamp(inputPayload.MoveDirection.z * _runSpeed, -_backpedalSpeed, _runSpeed)) * inputPayload.TickTime;

        _characterController.Move(desiredMovement);

        Vector3 velocity = (transform.position - previousPosition) / inputPayload.TickTime;

        if (isLocalPlayer)
            AnimateMovement(velocity);
        else if (isServer)
            RpcAnimateMovement(velocity);

        statePayload.Position = transform.position;
        statePayload.Velocity = velocity;
    }

    void AnimateMovement(Vector3 currentVelocity)
    {
        _animator.SetFloat(_forwardHash, transform.InverseTransformDirection(currentVelocity).z);
        _animator.SetFloat(_rightHash, transform.InverseTransformDirection(currentVelocity).x);
    }

    [ClientRpc(includeOwner = false)]
    void RpcAnimateMovement(Vector3 currentVelocity)
    {
        _animator.SetFloat(_forwardHash, transform.InverseTransformDirection(currentVelocity).z);
        _animator.SetFloat(_rightHash, transform.InverseTransformDirection(currentVelocity).x);
    }

    #endregion

    #region MONOBEHAVIOUR

    public override void Start()
    {
        _animator = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController>();

        base.Start();
    }

    #endregion

}
