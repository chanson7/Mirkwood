using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using Cinemachine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerObject : NetworkBehaviour
{
    #region EDITOR EXPOSED FIELDS

    [SerializeField] CinemachineVirtualCamera vcam;
    [SerializeField] Transform cameraPivot;
    [SerializeField] GameObject playerUI;

    #endregion

    public override void OnStartLocalPlayer()
    {
        playerUI.SetActive(true);

        GetComponent<PlayerInput>().enabled = true;
        Instantiate(vcam, transform).Follow = cameraPivot;
        
        Cursor.lockState = CursorLockMode.Locked;
    }
}
