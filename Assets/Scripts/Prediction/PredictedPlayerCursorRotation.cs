using UnityEngine;
using UnityEngine.InputSystem;

public class PredictedPlayerCursorRotation : PredictedTransformModule, IPredictedInputRecorder, IPredictedStateProcessor
{

    #region EDITOR EXPOSED FIELDS

    [Header("Player Preferences")]
    [SerializeField] float lateralSensitivity = 1f;
    [SerializeField] float verticalSensitivity = 1f;

    [Header("")]
    [SerializeField] float minPitchAngle = -15f;
    [SerializeField] float maxPitchAngle = 45f;
    [SerializeField] Transform cameraPivot;

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
        transform.Rotate(Vector3.up, inputPayload.LookAtDirection.x * inputPayload.TickDuration * lateralSensitivity);

        statePayload.Rotation = transform.rotation;
        statePayload.LookDirection = verticalRotation;
    }

    #endregion

}
