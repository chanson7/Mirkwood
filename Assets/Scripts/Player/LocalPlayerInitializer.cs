using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class LocalPlayerInitializer : MonoBehaviour
{

    [SerializeField] ScriptableEvent localPlayerStartedEvent;

    void Start()
    {
        this.GetComponent<PlayerInput>().enabled = true;

        //this event lets other game objects in the scene react to the local player starting.
        //example: the cinemachine camera sets the local player as its lookAt target
        localPlayerStartedEvent.Raise();
    }
}
