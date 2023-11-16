using UnityEngine;
using UnityEngine.InputSystem;

public class PredictedPlayerCursorRotation : PredictionModule, IPredictedInputRecorder, IPredictedInputProcessor
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
    Transform cameraTarget;

    #endregion

    #region FIELDS

    Vector2 rotationInput = Vector3.zero;

    #endregion

    #region METHODS

    void OnLook(InputValue input)
    {
        rotationInput.x = input.Get<Vector2>().x * lateralSensitivity;
        rotationInput.y = input.Get<Vector2>().y * verticalSensitivity;
    }

    public void RecordInput(ref InputPayload inputPayload)
    {
        inputPayload.LookAtDirection = rotationInput;
    }

    public void ProcessInput(ref StatePayload statePayload, InputPayload inputPayload)
    {
        float verticalRotation = statePayload.LookDirection - inputPayload.LookAtDirection.y * inputPayload.TickDuration;

        verticalRotation = Mathf.Clamp(verticalRotation, minPitchAngle, maxPitchAngle);

        cameraTarget.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        transform.Rotate(Vector3.up, inputPayload.LookAtDirection.x * inputPayload.TickDuration / 
            (statePayload.PlayerState.Equals(PlayerState.Attacking_Primary) ? attackRotationReduction : 1f)); //slow down rotation if we're attacking

        statePayload.Rotation = transform.rotation;
        statePayload.LookDirection = verticalRotation;
    }

    #endregion

}
