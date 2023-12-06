using UnityEngine;
using UnityEngine.InputSystem;

public class PredictedPlayerInput : MonoBehaviour
{

    [Header("Input Modifiers")]
    [SerializeField] float lateralRotationSensitivity = 1f;
    [SerializeField] float verticalRotationSensitivity = 1f;

    DuelistMovement predictedMovement;
    DuelistRotation predictedRotation;
    DuelistDodge predictedDodge;
    DuelistBlock predictedBlock;
    DuelistMeleeAttack predictedMeleeAttack;

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

    private void Awake()
    {
        predictedMovement = GetComponent<DuelistMovement>();
        predictedRotation = GetComponent<DuelistRotation>();
        predictedDodge =  GetComponent<DuelistDodge>();
        predictedBlock = GetComponent<DuelistBlock>();
        predictedMeleeAttack = GetComponent<DuelistMeleeAttack>();
    }

}
