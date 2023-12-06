using UnityEngine;

public class PredictedPlayerEnergy : DuelistControllerModule, IDuelistInputProcessor
{

    #region EDITOR EXPOSED FIELDS

    [Header("Energy Configuration")]
    [Tooltip("The base rate at which energy is recovered")]
    [SerializeField] float baseEnergyRecoveryRate = 3f;

    #endregion

    #region FIELDS

    DuelistBuild duelistBuild;

    #endregion

    public void ProcessInput(ref StatePayload statePayload, InputPayload inputPayload)
    {
        float energyRecoveryRate = baseEnergyRecoveryRate / duelistBuild.EnergyRecoveryRate;

        //player is below max energy
        if (statePayload.Energy < duelistBuild.MaxEnergy && statePayload.CombatState.Equals(CombatState.Balanced))
        {
            //player has waited long enough to recover 1 energy
            if (energyRecoveryRate < statePayload.LastEnergyRecoveryMs)
            {
                statePayload.Energy++;
                statePayload.LastEnergyRecoveryMs = 0f;
            }
            else
            {
                statePayload.LastEnergyRecoveryMs += inputPayload.TickDuration;
            }
        }
    }

    private void Awake()
    {
        duelistBuild = GetComponent<CombatantDuelist>().Build;
    }

}
