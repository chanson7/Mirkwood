using UnityEngine;
using Mirror;
using System;
public class PlayerEnergy : NetworkBehaviour
{

    uint maxEnergy = 10;
    [SyncVar][SerializeField] uint energy;

    [Tooltip("Time in seconds for the player to recover energy")]
    [SerializeField] float recoveryInterval = 4f;
    [Tooltip("Amount of energy recovered after each recovery interval")]
    [SerializeField] uint energyRecovered = 1;
    float lastRecoveryTime;

    public uint GetEnergy()
    {
        return energy;
    }

    public override void OnStartServer()
    {
        energy = maxEnergy;

        base.OnStartServer();
    }

    void Update()
    {
        if (isServer && Time.time - lastRecoveryTime > recoveryInterval)
        {
            RecoverEnergy(energyRecovered);
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
