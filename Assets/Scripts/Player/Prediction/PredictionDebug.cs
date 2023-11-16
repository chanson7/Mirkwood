using UnityEngine;
using Mirror;

[RequireComponent(typeof(PredictedCharacterController))]
public class PredictionDebug : NetworkBehaviour
{

    #region EDITOR EXPOSED FIELDS
    
    [SerializeField] GameObject ghostPrefab;
    [SerializeField] PredictionDebugUIController uiController;
    [SerializeField] bool enableGhost = true;

    #endregion

    #region FIELDS

    PredictedCharacterController predictedCharacterController;
    GameObject serverGhost;
    Animator ghostAnimator;

    static readonly int forwardHash = Animator.StringToHash("MoveForward");
    static readonly int rightHash = Animator.StringToHash("MoveRight");

    #endregion

    public override void OnStartLocalPlayer()
    {
        if (isActiveAndEnabled)
        {
            serverGhost = Instantiate(ghostPrefab);
            uiController = Instantiate(uiController, transform);
            ghostAnimator = serverGhost.GetComponent<Animator>();

            if(!enableGhost) { serverGhost.SetActive(false); }
        }
    }

    #region MONOBEHAVIOUR

    private void Awake()
    {
        predictedCharacterController = GetComponent<PredictedCharacterController>();
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            serverGhost.transform.SetPositionAndRotation(predictedCharacterController.LatestServerState.Position, predictedCharacterController.LatestServerState.Rotation);
            ghostAnimator.SetFloat(forwardHash, transform.InverseTransformDirection(predictedCharacterController.LatestServerState.Velocity).z);
            ghostAnimator.SetFloat(rightHash, transform.InverseTransformDirection(predictedCharacterController.LatestServerState.Velocity).x);
            uiController.UpdateTextUI(predictedCharacterController.LatestServerState);
        }
    }

    #endregion

}
