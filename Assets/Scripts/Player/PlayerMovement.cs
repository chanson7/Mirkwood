using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : NetworkBehaviour
{

    Vector2 movementInput;
    Vector2 rotationInput;
    Vector3 currentVelocity;
    [SyncVar] Vector2 knockbackValue = Vector2.zero;
    [SerializeField] CharacterController characterController;
    float knockbackTime = 0.4f;
    float verticalVelocity = 0f;

    [Header("Player Settings")]
    float xRotationSensitivity = 0.4f;
    public float movementSpeed;

    #region player inputs

    void Start()
    {
        characterController.enabled = hasAuthority || isServer;
    }

    void OnMove(InputValue input)
    {
        movementInput = input.Get<Vector2>();
    }

    void OnLook(InputValue input)
    {
        rotationInput = input.Get<Vector2>() * xRotationSensitivity;
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
        characterController.Move(direction * Time.fixedDeltaTime * movementSpeed);

    }

    void RotatePlayer(Vector2 rotation)
    {
        characterController.transform.Rotate(transform.up, rotation.x);
    }

    #endregion

}
