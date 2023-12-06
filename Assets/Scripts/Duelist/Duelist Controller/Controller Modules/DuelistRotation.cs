using UnityEngine;
using UnityEngine.InputSystem;

public class DuelistRotation : DuelistControllerModule, IDuelistInputRecorder, IDuelistInputProcessor
{

    #region EDITOR EXPOSED FIELDS

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
    Transform cameraTarget; //only used on a player duelist

    #endregion

    #region FIELDS

    Vector2 _rotationInput = Vector3.zero;
    float verticalRotation = 0f;

    #endregion

    #region PROPERTIES

    public Vector2 RotationInput { set { _rotationInput = value; } }
    public float RotationXInput { set { _rotationInput.x = value; } }

    #endregion

    #region METHODS

    public void RecordInput(ref InputPayload inputPayload)
    {
        inputPayload.HorizontalLookDirection = _rotationInput.x;
    }

    public void ProcessInput(ref StatePayload statePayload, InputPayload inputPayload)
    {
        transform.Rotate(Vector3.up, inputPayload.HorizontalLookDirection * inputPayload.TickDuration / 
            (statePayload.CombatState.Equals(CombatState.Attacking_Primary) ? attackRotationReduction : 1f)); //slow down rotation if we're attacking

        statePayload.Rotation = transform.rotation;
        //statePayload.LookDirection = verticalRotation;
    }

    #endregion

    private void Update()
    {
        if(isLocalPlayer)
        {
            verticalRotation -= _rotationInput.y * Time.deltaTime;

            verticalRotation = Mathf.Clamp(verticalRotation, minPitchAngle, maxPitchAngle);

            cameraTarget.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        }
    }

}
