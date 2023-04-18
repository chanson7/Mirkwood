using UnityEngine;
using System.Collections;
using Mirror;
using System;

public class PlayerEnergy : NetworkBehaviour
{

    uint maxEnergy = 100;
    [SerializeField] PlayerInterface playerInterface;
    bool isRecoveringEnergy = false;

    [SyncVar(hook = nameof(UpdateUserInterface))]
    [SerializeField] uint energy;

    [Tooltip("Time in seconds for the player to recover energy")]
    [SerializeField] float recoveryIntervalSeconds = 4f;
    [Tooltip("Amount of energy recovered after each recovery interval")]
    [SerializeField] uint energyRecoveredPerInterval = 1;
    public uint GetEnergy() { return energy; }

    public override void OnStartServer()
    {
        energy = maxEnergy;

        base.OnStartServer();
    }

    [Server]
    public bool SpendEnergy(uint energySpent)
    {
        if (energy < energySpent)
        {
            Debug.Log($"..{this.name} does not have enough energy");
            return false;
        }
        else
        {
            energy -= energySpent;
            Math.Clamp(energy, 0, maxEnergy);

            if (!isRecoveringEnergy)
                StartCoroutine(RecoverEnergy(energyRecoveredPerInterval));

            Debug.Log($"..{this.name} spends {energySpent} energy and now has {energy}");

            return true;
        }

    }

    [Server]
    public void LoseEnergy(uint energyLost)
    {
        energy -= energyLost;

        Math.Clamp(energy, 0, maxEnergy);

        if (!isRecoveringEnergy)
            StartCoroutine(RecoverEnergy(energyRecoveredPerInterval));

        Debug.Log($"..{this.name} loses {energyLost} energy and now has {energy}");
    }

    [Server]
    IEnumerator RecoverEnergy(uint energyRecovered)
    {
        isRecoveringEnergy = true;
        while (energy < maxEnergy)
        {
            yield return new WaitForSeconds(recoveryIntervalSeconds);
            energy += energyRecovered;
            Math.Clamp(energy, 0, maxEnergy);
        }
        isRecoveringEnergy = false;
    }

    void UpdateUserInterface(uint oldEnergyValue, uint newEnergyValue)
    {
        playerInterface.SetEnergyText(newEnergyValue);
    }
}
