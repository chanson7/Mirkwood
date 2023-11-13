using UnityEngine;

public class PredictedPlayerEnergy : PredictedTransformModule, IPredictedInputProcessor
{

    #region EDITOR EXPOSED FIELDS

    [Header("Energy Configuration")]
    [Tooltip("The base rate at which energy is recovered")]
    [SerializeField] float baseEnergyRecoveryRate = 3f;

    #endregion

    #region FIELDS

    PlayerBuildDefinition playerClass;    

    #endregion

    #region PROPERTIES

    #endregion

    public void ProcessInput(ref StatePayload statePayload, InputPayload inputPayload)
    {
        float energyRecoveryRate = baseEnergyRecoveryRate / playerClass.EnergyRecoveryRate;

        //player is below max energy
        if (statePayload.Energy < playerClass.MaxEnergy)
        {
            //player has waited long enough to recover 1 energy
            if (energyRecoveryRate < statePayload.LastEnergyRecoveryMs)
            {
                Debug.Log($"Energy Recovered! Player has {statePayload.Energy} energy");
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
        playerClass = GetComponent<PlayerObject>().PlayerBuild;
    }

}
