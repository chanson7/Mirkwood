using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(NetworkTransform))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerMovement : NetworkBehaviour
{

    Vector2 movementInput;
    Vector2 rotationInput;
    [SerializeField] CharacterController characterController;
    [SerializeField] PlayerInput playerInput;

    float verticalVelocity = 0f;

    #region player inputs

    void Start()
    {
        if (!hasAuthority && !isServer)
        {
            characterController.enabled = false;
            playerInput.enabled = false;
            this.enabled = false;
        }

    }

    void OnMove(InputValue input)
    {
        movementInput = input.Get<Vector2>();
    }

    void OnLook(InputValue input)
    {
        rotationInput = input.Get<Vector2>();
    }
    #endregion

    private void FixedUpdate()
    {
        if (!base.hasAuthority)
            return;

        CmdMovePlayer(movementInput);
        CmdRotatePlayer(rotationInput);

        MovePlayer(movementInput);
        RotatePlayer(rotationInput);
    }

    [Command]
    void CmdMovePlayer(Vector2 movementInput)
    {
        MovePlayer(movementInput);
    }

    [Command]
    void CmdRotatePlayer(Vector2 rotationInput)
    {
        RotatePlayer(rotationInput);
    }

    #region transform logic
    void MovePlayer(Vector2 movement)
    {

        if (!characterController.isGrounded)
            verticalVelocity += Physics.gravity.y;
        else
            verticalVelocity = Physics.gravity.y;

        Vector3 direction = new Vector3(movement.x, verticalVelocity, movement.y);

        direction = transform.TransformDirection(direction);
        characterController.Move(direction * Time.fixedDeltaTime);

    }

    void RotatePlayer(Vector2 rotation)
    {
        characterController.transform.Rotate(transform.up, rotation.x);
    }

    #endregion

}
