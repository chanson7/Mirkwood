using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

[RequireComponent(typeof(CharacterController))]
public class PredictedPlayerMovement : PredictedPlayerInputProcessor
{
    Vector3 currentVelocity;
    public bool isSprinting = false;
    public Vector3 movementInput = new Vector3();
    [SerializeField] float movementSpeed;
    [SerializeField] float sprintSpeedMultiplier;
    [SerializeField] CharacterController characterController;
    [SerializeField] Animator animator;

    static int forwardHash = Animator.StringToHash("Forward");
    static int rightHash = Animator.StringToHash("Right");

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

    void OnSprint(InputValue input)
    {
        isSprinting = input.isPressed;
    }

    public override InputPayload GatherClientInput(InputPayload movementPayload)
    {
        movementPayload.MovementSpeedMultiplier = isSprinting ? sprintSpeedMultiplier : 1f;
        movementPayload.MoveDirection = movementInput;

        return movementPayload;
    }

    public override StatePayload ProcessTick(StatePayload statePayload, InputPayload inputPayload)
    {

        //clamp protects against somebody boosting the movementSpeedMultiplier.  Will need more thorough checks if later on there are different things than sprint that can effect movement speed
        //normalizing movedirection so they can't mess with the value to change their speed
        currentVelocity = Vector3.Lerp(currentVelocity, inputPayload.MoveDirection.normalized * movementSpeed * Mathf.Clamp(inputPayload.MovementSpeedMultiplier, 0f, sprintSpeedMultiplier), 0.2f);

        currentVelocity.y = characterController.isGrounded ? Physics.gravity.y : currentVelocity.y + Physics.gravity.y;

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
