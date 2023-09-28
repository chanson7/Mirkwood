using UnityEngine;
using Mirror;

[RequireComponent(typeof(PredictedPlayerTransform))]
public class PredictionDebug : NetworkBehaviour
{

    #region EDITOR EXPOSED FIELDS
    
    [SerializeField] GameObject _ghostPrefab;
    [SerializeField] PredictionDebugUIController _uiController;

    #endregion

    PredictedPlayerTransform _predictedPlayerTransform;
    GameObject _serverGhost;
    Animator _ghostAnimator;

    static readonly int _forwardHash = Animator.StringToHash("Forward");
    static readonly int _rightHash = Animator.StringToHash("Right");


    public override void OnStartLocalPlayer()
    {
        if (isActiveAndEnabled)
        {
            _serverGhost = Instantiate(_ghostPrefab);
            _uiController = Instantiate(_uiController, transform);
            _ghostAnimator = _serverGhost.GetComponent<Animator>();
        }
    }

    #region MONOBEHAVIOUR

    private void Awake()
    {
        _predictedPlayerTransform = GetComponent<PredictedPlayerTransform>();
    }
    void Update()
    {
        if (isLocalPlayer)
        {
            _serverGhost.transform.SetPositionAndRotation(_predictedPlayerTransform.LatestServerState.Position, _predictedPlayerTransform.LatestServerState.Rotation);
            _ghostAnimator.SetFloat(_forwardHash, transform.InverseTransformDirection(_predictedPlayerTransform.LatestServerState.Velocity).z);
            _ghostAnimator.SetFloat(_rightHash, transform.InverseTransformDirection(_predictedPlayerTransform.LatestServerState.Velocity).x);
            _uiController.UpdateTextUI(_predictedPlayerTransform.LatestServerState);
        }
    }

    #endregion

}
