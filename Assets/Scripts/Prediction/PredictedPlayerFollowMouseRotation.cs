using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

public class PredictedPlayerFollowMouseRotation : PredictedPlayerTickProcessor
{
    Ray pointerRay;
    [SerializeField] LayerMask pointerMask; //so that the player will not look at everything the pointer ray hits
    public Vector3 mouseWorldPosition = new Vector3();
    [SerializeField] float rotateSpeed = 8f;

    private void Update()
    {
        if (isLocalPlayer)
            ClientRotateTowardsMouse();
    }

    [Client]
    void ClientRotateTowardsMouse()
    {
        pointerRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray: pointerRay, layerMask: pointerMask, maxDistance: 100f, hitInfo: out RaycastHit hit))
        {
            mouseWorldPosition = hit.point;
            mouseWorldPosition.y = transform.position.y; //transform should not rotate on Y axis
        }
    }

    public override InputPayload GatherInput(InputPayload inputPayload)
    {
        inputPayload.LookAtDirection = mouseWorldPosition;

        return inputPayload;
    }

    public override StatePayload ProcessTick(StatePayload statePayload, InputPayload inputPayload)
    {
        if (inputPayload.ActiveAnimationPriority < AnimationPriority.None)
            return statePayload; //don't do any processing if there is an active animation

        Vector3 direction = inputPayload.LookAtDirection - transform.position;

        Quaternion desiredRotation = Quaternion.LookRotation(direction);

        // Interpolate the player's rotation towards the desired rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotateSpeed * (1f / MirkwoodNetworkManager.singleton.serverTickRate));

        statePayload.Rotation = transform.rotation;

        return statePayload;
    }

}
