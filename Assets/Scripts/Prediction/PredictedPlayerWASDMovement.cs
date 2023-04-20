using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class PredictedPlayerWASDMovement : PredictedPlayerTickProcessor
{
    Vector3 currentVelocity;
    public bool isWalking = false;
    public Vector3 movementInput = new Vector3();
    [SerializeField] float movementSpeed;
    [SerializeField] float walkSpeedMultiplier;
    [SerializeField] Animator animator;
    CharacterController characterController;

    static int forwardHash = Animator.StringToHash("Forward");
    static int rightHash = Animator.StringToHash("Right");

    public override void Start()
    {
        characterController = gameObject.GetComponent<CharacterController>();

        base.Start();
    }

    void Update()
    {
        if (isLocalPlayer)
            AnimateMovement(currentVelocity);
        else if (isServer)
            RpcAnimateMovement(currentVelocity);
    }

    void OnMove(InputValue input)
    {
        movementInput.x = input.Get<Vector2>().x;
        movementInput.z = input.Get<Vector2>().y;
    }

    void OnWalk(InputValue input)
    {
        isWalking = input.isPressed;
    }

    public override InputPayload GatherInput(InputPayload inputPayload)
    {
        inputPayload.IsWalking = isWalking;
        inputPayload.MoveDirection = movementInput;

        return inputPayload;
    }

    public override StatePayload ProcessTick(StatePayload statePayload, InputPayload inputPayload)
    {
        if (inputPayload.ActiveAction < PlayerAnimationEvent.None)
            return statePayload; //don't do any processing if there is an active animation

        currentVelocity.y = characterController.isGrounded ? Physics.gravity.y : currentVelocity.y + Physics.gravity.y;
        currentVelocity = Vector3.Lerp(currentVelocity, inputPayload.MoveDirection.normalized * movementSpeed * (inputPayload.IsWalking ? walkSpeedMultiplier : 1f), 0.2f);

        Vector3 movementValue = currentVelocity * (1f / MirkwoodNetworkManager.singleton.serverTickRate);

        characterController.Move(movementValue);

        statePayload.Position = transform.position;
        statePayload.CurrentVelocity = characterController.velocity;

        return statePayload;
    }

    void AnimateMovement(Vector3 currentVelocity)
    {
        animator.SetFloat(forwardHash, transform.InverseTransformDirection(currentVelocity).z);
        animator.SetFloat(rightHash, transform.InverseTransformDirection(currentVelocity).x);
    }

    [ClientRpcAttribute(includeOwner = false)]
    void RpcAnimateMovement(Vector3 currentVelocity)
    {
        animator.SetFloat(forwardHash, transform.InverseTransformDirection(currentVelocity).z);
        animator.SetFloat(rightHash, transform.InverseTransformDirection(currentVelocity).x);
    }

}
