using UnityEngine;
using UnityEngine.InputSystem;

public class PredictedPlayerCursorRotation : PredictedTransformModule, IPredictedInputRecorder, IPredictedStateProcessor
{

    #region EDITOR EXPOSED FIELDS

    [Header("Player Preferences")]
    [SerializeField] float _lateralSensitivity = 1f;
    [SerializeField] float _verticalSensitivity = 1f;

    [Header("")]
    [SerializeField] float _minPitchAngle = -15f;
    [SerializeField] float _maxPitchAngle = 45f;
    [SerializeField] Transform _cameraPivot;

    #endregion

    #region FIELDS

    Vector2 _rotationInput = Vector3.zero;

    #endregion

    #region METHODS

    void OnLook(InputValue input)
    {
        _rotationInput.x = input.Get<Vector2>().x;
        _rotationInput.y = input.Get<Vector2>().y;
    }

    public void RecordInput(ref InputPayload inputPayload)
    {
        inputPayload.LookAtDirection = _rotationInput;
    }

    public void ProcessTick(ref StatePayload statePayload, InputPayload inputPayload)
    {
        float verticalRotation = statePayload.LookDirection - inputPayload.LookAtDirection.y * _verticalSensitivity * inputPayload.TickTime;

        verticalRotation = Mathf.Clamp(verticalRotation, _minPitchAngle, _maxPitchAngle);

        _cameraPivot.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        transform.Rotate(Vector3.up, inputPayload.LookAtDirection.x * inputPayload.TickTime * _lateralSensitivity);

        statePayload.Rotation = transform.rotation;
        statePayload.LookDirection = verticalRotation;
    }

    #endregion

}
