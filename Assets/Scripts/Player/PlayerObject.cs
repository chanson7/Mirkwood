using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using Cinemachine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerObject : NetworkBehaviour
{
    #region EDITOR EXPOSED FIELDS

    [SerializeField] CinemachineVirtualCamera virtualCamera;
    [SerializeField] Transform cameraTargetTransform;
    [SerializeField] GameObject playerUI;

    #endregion

    public override void OnStartLocalPlayer()
    {
        playerUI.SetActive(true);

        GetComponent<PlayerInput>().enabled = true;
        Instantiate(virtualCamera, transform).Follow = cameraTargetTransform;

        Cursor.lockState = CursorLockMode.Locked;
    }

}
