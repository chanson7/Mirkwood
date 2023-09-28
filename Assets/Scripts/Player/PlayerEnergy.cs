using UnityEngine;
using System.Collections;
using Mirror;
using System;

public class PlayerEnergy : NetworkBehaviour
{

    #region EDITOR EXPOSED FIELDS

    [SerializeField] PlayerUIController playerUIController;
    [SyncVar(hook = nameof(UpdateUserInterface))]
    [SerializeField] int _energy;
    [SerializeField] int _maxEnergy;
    [Tooltip("Time in seconds for the player to recover energy")]
    [SerializeField] float _recoveryIntervalSeconds = 4f;
    [Tooltip("Amount of energy recovered after each recovery interval")]
    [SerializeField] int _energyRecoveredPerInterval = 1;
    
    #endregion

    #region FIELDS

    bool _isRecoveringEnergy = false;

    #endregion

    #region PROPERTIES

    public int Energy { get { return _energy; } }

    #endregion

    public override void OnStartServer()
    {
        _energy = _maxEnergy;

        base.OnStartServer();
    }

    [Server]
    public bool SpendEnergy(int energySpent)
    {
        if (_energy < energySpent)
        {
            Debug.Log($"..{this.name} does not have enough energy");
            return false;
        }
        else
        {
            _energy = Math.Clamp(_energy -= energySpent, 0, _maxEnergy);

            if (!_isRecoveringEnergy)
                StartCoroutine(RecoverEnergy(_energyRecoveredPerInterval));

            Debug.Log($"..{this.name} spends {energySpent} energy and now has {_energy}");

            return true;
        }

    }

    [Server]
    public void LoseEnergy(int energyLost)
    {
        _energy -= energyLost;

        Math.Clamp(_energy, 0, _maxEnergy);

        if (!_isRecoveringEnergy)
            StartCoroutine(RecoverEnergy(_energyRecoveredPerInterval));

        Debug.Log($"..{this.name} loses {energyLost} energy and now has {_energy}");
    }

    [Server]
    IEnumerator RecoverEnergy(int energyRecovered)
    {
        _isRecoveringEnergy = true;
        while (_energy < _maxEnergy)
        {
            yield return new WaitForSeconds(_recoveryIntervalSeconds);
            _energy += energyRecovered;
            Math.Clamp(_energy, 0, _maxEnergy);
        }
        _isRecoveringEnergy = false;
    }

    void UpdateUserInterface(int oldEnergyValue, int newEnergyValue)
    {
        playerUIController.SetEnergyText(newEnergyValue);
    }
}
