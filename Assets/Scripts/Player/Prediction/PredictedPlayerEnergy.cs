using UnityEngine;

public class PredictedPlayerEnergy : PredictionModule, IPredictedInputProcessor
{

    #region EDITOR EXPOSED FIELDS

    [Header("Energy Configuration")]
    [Tooltip("The base rate at which energy is recovered")]
    [SerializeField] float baseEnergyRecoveryRate = 3f;

    #endregion

    #region FIELDS

    PlayerBuildDefinition playerClass;

    #endregion

    public void ProcessInput(ref StatePayload statePayload, InputPayload inputPayload)
    {
        float energyRecoveryRate = baseEnergyRecoveryRate / playerClass.EnergyRecoveryRate;

        //player is below max energy
        if (statePayload.Energy < playerClass.MaxEnergy && statePayload.PlayerState.Equals(PlayerState.Balanced))
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
        playerClass = GetComponent<PlayerDuelistObject>().PlayerBuild;
    }

}
