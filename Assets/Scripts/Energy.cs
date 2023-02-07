using UnityEngine;
using Mirror;
using System;
public class Energy : NetworkBehaviour
{

    [SyncVar] int maxEnergy = 10;
    [SyncVar] uint energy;

    [Tooltip("Time in seconds for the player to recover 1 energy")]
    [SerializeField] float recoveryTime = 4f;
    float lastRecoveryTime;

    void Update()
    {
        if (isServer && Time.time - lastRecoveryTime > recoveryTime)
        {
            RecoverEnergy(1);
            lastRecoveryTime = Time.time;
        }
    }

    [Server]
    public bool SpendEnergy(uint energySpent)
    {
        if (energy < energySpent)
        {
            Debug.Log($"..{this.name} does not have {energySpent} energy");
            return false;
        }
        else
        {
            energy -= energySpent;
            Math.Clamp(energy, 0, maxEnergy);
            Debug.Log($"..{this.name} spends {energySpent} and now has {energy} energy");

            return true;
        }

    }

    [Server]
    public void RecoverEnergy(uint energyRecovered)
    {
        if (energy < maxEnergy)
        {
            energy += energyRecovered;
            Math.Clamp(energy, 0, maxEnergy);
            Debug.Log($"..{this.name} recovers {energyRecovered} and now has {energy} energy");
        }
    }

}
