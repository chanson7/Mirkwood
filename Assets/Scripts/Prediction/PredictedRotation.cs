using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

public class PredictedRotation : NetworkBehaviour
{
    Ray pointerRay;
    [SerializeField] LayerMask pointerMask; //so that the player will not look at everything the pointer ray hits
    public Vector3 mouseWorldPosition = new Vector3();

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

    public StatePayload ProcessRotationInput(StatePayload statePayload, Vector3 mouseWorldPosition)
    {
        transform.LookAt(mouseWorldPosition, Vector3.up); //todo need to use rigidbody to rotate here? 
        statePayload.Rotation = transform.rotation;

        return statePayload;
    }

}
