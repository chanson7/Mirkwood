using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerDuelist : CombatantDuelist
{
    #region EDITOR EXPOSED FIELDS

    [Header("Input Modifiers")]
    [SerializeField] float lateralRotationSensitivity = 1f;
    [SerializeField] float verticalRotationSensitivity = 1f;

    [Header("")]
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    [SerializeField] Transform cameraTargetTransform;
    [SerializeField] GameObject playerUI;

    #endregion

    #region FIELDS

    #endregion

    #region INPUT

    void OnMove(InputValue input)
    {
        Vector2 movementInput = new(input.Get<Vector2>().x, input.Get<Vector2>().y);

        predictedMovement.MovementInput = movementInput;
    }

    void OnLook(InputValue input)
    {
        Vector2 rotationInput = new(input.Get<Vector2>().x * lateralRotationSensitivity, input.Get<Vector2>().y * verticalRotationSensitivity);

        predictedRotation.RotationInput = rotationInput;
    }

    void OnDodge(InputValue input)
    {
        predictedDodge.IsDodgeButtonPressed = input.isPressed;
    }

    void OnAttack(InputValue input)
    {
        predictedMeleeAttack.IsAttackButtonPressed = input.isPressed;
    }

    void OnBlock(InputValue input)
    {
        predictedBlock.IsBlockButtonPressed = input.isPressed;
    }

    #endregion

    public override void OnStartLocalPlayer()
    {
        playerUI.SetActive(true);

        GetComponent<PlayerInput>().enabled = true;
        Instantiate(virtualCamera, transform).Follow = cameraTargetTransform;

        Cursor.lockState = CursorLockMode.Locked;
    }

}
