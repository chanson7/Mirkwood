using UnityEngine;
using Mirror;

public class CombatantDuelist : NetworkBehaviour
{

    [Header("Duelist Build")]
    [SerializeField] DuelistBuild _build;

    #region PROPERTIES

    public DuelistBuild Build { get { return _build; } }

    #endregion

    protected PredictedPlayerMovement predictedMovement;
    protected PredictedPlayerCursorRotation predictedRotation;
    protected PredictedPlayerDodge predictedDodge;
    protected PredictedPlayerBlock predictedBlock;
    protected PredictedPlayerMeleeAttack predictedMeleeAttack;

    private void Awake()
    {
        predictedMovement = GetComponent<PredictedPlayerMovement>();
        predictedRotation = GetComponent<PredictedPlayerCursorRotation>();
        predictedDodge = GetComponent<PredictedPlayerDodge>();
        predictedBlock = GetComponent<PredictedPlayerBlock>();
        predictedMeleeAttack = GetComponent<PredictedPlayerMeleeAttack>();
    }

    public override void OnStartServer()
    {
        DuelGameManager.Singleton.RegisterDuelist(this);
    }
}
