using UnityEngine;
using Mirror;

public class CombatantDuelist : NetworkBehaviour
{

    [Header("Duelist Build")]
    [SerializeField] DuelistBuild _build;

    #region PROPERTIES

    public DuelistBuild Build { get { return _build; } }

    #endregion

    protected DuelistMovement movement;
    protected DuelistRotation rotation;
    protected DuelistDodge dodge;
    protected DuelistBlock block;
    protected DuelistMeleeAttack meleeAttack;

    private void Awake()
    {
        movement = GetComponent<DuelistMovement>();
        rotation = GetComponent<DuelistRotation>();
        dodge = GetComponent<DuelistDodge>();
        block = GetComponent<DuelistBlock>();
        meleeAttack = GetComponent<DuelistMeleeAttack>();
    }

    public override void OnStartServer()
    {
        DuelGameManager.Singleton.RegisterDuelist(this);
    }
}
