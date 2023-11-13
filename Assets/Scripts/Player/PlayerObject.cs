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

    [Header("Player Build")]
    [SerializeField] PlayerBuildDefinition _playerBuild;

    #endregion

    #region PROPERTIES

    public PlayerBuildDefinition PlayerBuild {  get { return _playerBuild; } }

    #endregion

    public override void OnStartServer()
    {
        GameManager.Singleton.RegisterPlayerObject(this);
    }

    public override void OnStartLocalPlayer()
    {
        playerUI.SetActive(true);

        GetComponent<PlayerInput>().enabled = true;
        Instantiate(virtualCamera, transform).Follow = cameraTargetTransform;

        Cursor.lockState = CursorLockMode.Locked;
    }

}
