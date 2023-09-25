using UnityEngine;
using UnityEngine.InputSystem;

public class PredictedPlayerCursorRotation : PredictedStateProcessor, IPredictedInputProcessor
{

    [SerializeField] float _maxPitchAngle = 45f;
    [SerializeField] float _minPitchAngle = -45f;
    [SerializeField] Transform _cameraPivot;

    Vector2 _rotationInput = Vector3.zero;
    void OnLook(InputValue input)
    {
        _rotationInput.x = input.Get<Vector2>().x;
        _rotationInput.y = input.Get<Vector2>().y;
    }
    public void GatherInput(ref InputPayload inputPayload)
    {
        inputPayload.LookAtDirection = _rotationInput;
    }
    public override void ProcessTick(ref StatePayload statePayload, InputPayload inputPayload)
    {
        //_verticalRotation -= _pitch;
        //_verticalRotation = Mathf.Clamp(_verticalRotation, _minPitchAngle, _maxPitchAngle);

        //_cameraPivot.localRotation = Quaternion.Euler(Mathf.Clamp(-inputPayload.LookAtDirection.y, _minPitchAngle, _maxPitchAngle), 0f, 0f);
        transform.Rotate(Vector3.up, inputPayload.LookAtDirection.x * inputPayload.TickTime);

        statePayload.Rotation = transform.rotation;
    }

}
