using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class PredictedPlayerTarget : PredictedTransformModule, IPredictedInputRecorder, IPredictedStateProcessor
{

    #region EDITOR EXPOSED FIELDS

    [Header("Targeting Settings")]
    [SerializeField] float sphereCastRadius = 1.0f;
    [SerializeField] float TargetingRange = 20f;
    [SerializeField] LayerMask targetableLayers;
    [SerializeField] Transform cameraTarget;

    #endregion

    #region FIELDS

    bool targetButtonPressedThisTick = false;
    Transform _targetMarker;

    #endregion

    #region PROPERTIES

    public Transform TargetMarker { set { _targetMarker = value; } }

    #endregion

    #region INPUT

    void OnTarget(InputValue input)
    {
        targetButtonPressedThisTick = input.isPressed;
    }

    #endregion

    public void RecordInput(ref InputPayload inputPayload)
    {
        inputPayload.TargetPressed = targetButtonPressedThisTick;
        targetButtonPressedThisTick = false;
    }

    public void ProcessTick(ref StatePayload statePayload, InputPayload inputPayload)
    {
        if (inputPayload.TargetPressed)
        {
            //already have a target
            if(statePayload.TargetPosition != null)
            {
                statePayload.TargetPosition = transform.position;

                if (isLocalPlayer) 
                    _targetMarker.transform.position = cameraTarget.position;

                return;
            }

            //there is an object in front of us
            if (Physics.SphereCast(transform.position, sphereCastRadius, transform.forward, out RaycastHit hit, TargetingRange))
            {
                //it is targetable
                if (targetableLayers == (targetableLayers | (1 << hit.transform.gameObject.layer)))
                {
                    _targetMarker.gameObject.SetActive(true);
                    _targetMarker.position = hit.transform.position;

                    statePayload.TargetPosition = hit.transform.position;    
                }
            }
        }
    }

}
