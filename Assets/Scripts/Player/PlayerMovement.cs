using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(NetworkTransform))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerMovement : NetworkBehaviour
{

    Vector2 movementInput;
    Vector3 mouseWorldPosition;
    [SerializeField] CharacterController characterController;
    [SerializeField] PlayerInput playerInput;

    float verticalVelocity = 0f;
    [SerializeField] float movementSpeed = 5f;

    #region player inputs

    void OnMove(InputValue input)
    {
        movementInput = input.Get<Vector2>();
    }

    void OnLook()
    {
        mouseWorldPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
    }
    #endregion

    private void FixedUpdate()
    {
        if (!base.hasAuthority)
            return;

        CmdMovePlayer(movementInput);
        CmdRotatePlayer(mouseWorldPosition);

        // MovePlayer(movementInput);
        // RotatePlayer(mouseWorldPosition);
    }

    [Command]
    void CmdMovePlayer(Vector2 movementInput)
    {
        MovePlayer(movementInput);
    }

    [Command]
    void CmdRotatePlayer(Vector3 mouseWorldPosition)
    {
        RotatePlayer(mouseWorldPosition);
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
        characterController.Move(direction * Time.fixedDeltaTime * movementSpeed);

    }

    void RotatePlayer(Vector3 mouseWorldPosition)
    {
        // Quaternion rotation = Quaternion.LookRotation(mouseWorldPosition - transform.position);
        // transform.rotation = rotation;
    }

    #endregion

}
