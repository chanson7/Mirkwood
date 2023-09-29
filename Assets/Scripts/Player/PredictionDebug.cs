using UnityEngine;
using Mirror;

[RequireComponent(typeof(PredictedPlayerTransform))]
public class PredictionDebug : NetworkBehaviour
{

    #region EDITOR EXPOSED FIELDS
    
    [SerializeField] GameObject ghostPrefab;
    [SerializeField] PredictionDebugUIController uiController;

    #endregion

    PredictedPlayerTransform predictedPlayerTransform;
    GameObject serverGhost;
    Animator ghostAnimator;

    static readonly int forwardHash = Animator.StringToHash("Forward");
    static readonly int rightHash = Animator.StringToHash("Right");


    public override void OnStartLocalPlayer()
    {
        if (isActiveAndEnabled)
        {
            serverGhost = Instantiate(ghostPrefab);
            uiController = Instantiate(uiController, transform);
            ghostAnimator = serverGhost.GetComponent<Animator>();
        }
    }

    #region MONOBEHAVIOUR

    private void Awake()
    {
        predictedPlayerTransform = GetComponent<PredictedPlayerTransform>();
    }
    void Update()
    {
        if (isLocalPlayer)
        {
            serverGhost.transform.SetPositionAndRotation(predictedPlayerTransform.LatestServerState.Position, predictedPlayerTransform.LatestServerState.Rotation);
            ghostAnimator.SetFloat(forwardHash, transform.InverseTransformDirection(predictedPlayerTransform.LatestServerState.Velocity).z);
            ghostAnimator.SetFloat(rightHash, transform.InverseTransformDirection(predictedPlayerTransform.LatestServerState.Velocity).x);
            uiController.UpdateTextUI(predictedPlayerTransform.LatestServerState);
        }
    }

    #endregion

}
