using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class PredictedPlayerMovement : PredictedStateProcessor, IPredictedInputProcessor
{
    #region EDITOR EXPOSED FIELDS

    [SerializeField] float _movementSpeed;
    [SerializeField] float _walkSpeedMultiplier;
    Animator _animator;

    #endregion

    #region FIELDS

    Vector3 _movementInput = Vector3.zero;
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

    public void GatherInput(ref InputPayload inputPayload)
    {
        inputPayload.MoveDirection = _movementInput;
    }

    public override void ProcessTick(ref StatePayload statePayload, InputPayload inputPayload)
    {
        Vector3 moveDirection = transform.right * inputPayload.MoveDirection.x + transform.forward * inputPayload.MoveDirection.z;
        moveDirection.y = _characterController.isGrounded ? 0f : (statePayload.CurrentVelocity.y + Physics.gravity.y);

        _characterController.Move(moveDirection * inputPayload.TickTime);

        statePayload.Position = transform.position;
        statePayload.CurrentVelocity = _characterController.velocity;
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
        _characterController = gameObject.GetComponent<CharacterController>();
        _animator = gameObject.GetComponent<Animator>();

        base.Start();
    }

    void Update()
    {
        if (isLocalPlayer)
            AnimateMovement(_characterController.velocity);
        else if (isServer)
            RpcAnimateMovement(_characterController.velocity);
    }

    #endregion

}
