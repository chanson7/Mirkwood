using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class LocalPlayerInitializer : NetworkBehaviour
{

    [SerializeField] ScriptableEvent localPlayerStartedEvent;
    [SerializeField] PlayerInput playerInput;

    public override void OnStartLocalPlayer()
    {
        playerInput.enabled = true;

        //this event lets other game objects in the scene react to the local player starting.
        //example: the cinemachine camera sets the local player as its lookAt target
        localPlayerStartedEvent.Raise();
    }
}
