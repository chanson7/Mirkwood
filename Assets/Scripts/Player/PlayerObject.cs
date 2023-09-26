using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using Cinemachine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerObject : NetworkBehaviour
{
    #region EDITOR EXPOSED FIELDS

    [SerializeField] CinemachineVirtualCamera _vcam;
    [SerializeField] Transform _cameraPivot;

    #endregion

    public override void OnStartLocalPlayer()
    {
        GetComponent<PlayerInput>().enabled = true;
        Instantiate(_vcam, transform).Follow = _cameraPivot;
        
        Cursor.lockState = CursorLockMode.Locked;
    }
}
