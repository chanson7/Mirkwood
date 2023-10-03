using UnityEngine;
using UnityEngine.InputSystem;

public class PredictedPlayerCursorRotation : PredictedTransformModule, IPredictedInputRecorder, IPredictedStateProcessor
{

    #region EDITOR EXPOSED FIELDS

    [Header("Player Preferences")]
    [SerializeField] float lateralSensitivity = 1f;
    [SerializeField] float verticalSensitivity = 1f;

    [Header("Rotation Settings")]
    [SerializeField] 
    float minPitchAngle = -15f;
    [SerializeField] 
    float maxPitchAngle = 45f;

    [Range(1f, 10f)]
    [Tooltip("A multiplier that slows down rotation speed during attacks.")]
    [SerializeField] 
    float attackRotationReduction = 1f;

    [SerializeField] 
    Transform cameraPivot;

    #endregion

    #region FIELDS

    Vector2 rotationInput = Vector3.zero;

    #endregion

    #region METHODS

    void OnLook(InputValue input)
    {
        rotationInput.x = input.Get<Vector2>().x;
        rotationInput.y = input.Get<Vector2>().y;
    }

    public void RecordInput(ref InputPayload inputPayload)
    {
        inputPayload.LookAtDirection = rotationInput;
    }

    public void ProcessTick(ref StatePayload statePayload, InputPayload inputPayload)
    {
        float verticalRotation = statePayload.LookDirection - inputPayload.LookAtDirection.y * verticalSensitivity * inputPayload.TickDuration;

        verticalRotation = Mathf.Clamp(verticalRotation, minPitchAngle, maxPitchAngle);

        cameraPivot.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        transform.Rotate(Vector3.up, inputPayload.LookAtDirection.x * inputPayload.TickDuration * lateralSensitivity / 
            (statePayload.PlayerState.Equals(PlayerState.Attack1) ? attackRotationReduction : 1f)); //slow down rotation if we're attacking

        statePayload.Rotation = transform.rotation;
        statePayload.LookDirection = verticalRotation;
    }

    #endregion

}
