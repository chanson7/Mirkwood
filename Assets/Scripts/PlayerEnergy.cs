using UnityEngine;
using System.Collections;
using Mirror;
using System;
public class PlayerEnergy : NetworkBehaviour
{

    uint maxEnergy = 10;
    bool isRecoveringEnergy = false;
    [SyncVar][SerializeField] uint energy;

    [Tooltip("Time in seconds for the player to recover energy")]
    [SerializeField] float recoveryInterval = 4f;
    [Tooltip("Amount of energy recovered after each recovery interval")]
    [SerializeField] uint energyRecovered = 1;
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
                StartCoroutine(RecoverEnergy(energyRecovered));

            Debug.Log($"..{this.name} spends {energySpent} and now has {energy} energy");

            return true;
        }

    }

    [Server]
    IEnumerator RecoverEnergy(uint energyRecovered)
    {
        isRecoveringEnergy = true;
        while (energy < maxEnergy)
        {
            yield return new WaitForSeconds(recoveryInterval);
            energy += energyRecovered;
            Math.Clamp(energy, 0, maxEnergy);
        }
        isRecoveringEnergy = false;
    }
}
