using UnityEngine;
using System.Collections;
using Mirror;
using System;

public class PlayerBalance : NetworkBehaviour
{

    #region EDITOR EXPOSED FIELDS

    [SerializeField] PlayerUIController playerUIController;
    [SyncVar(hook = nameof(UpdateUserInterface))]
    [SerializeField] int _balance = 100;
    [Tooltip("Time in seconds for the player to recover balance")]
    [SerializeField] float _recoveryInterval;
    [Tooltip("Amount of balance recovered after each recovery interval")]
    [SerializeField] int _balanceRecoveredPerInterval;
    [Tooltip("Seconds before balance recovery begins")]
    [SerializeField] float _recoveryDelay;

    #endregion

    #region FIELDS

    int _maxBalance = 100;
    float _timeOfLastBalanceLoss = 0f;
    bool _isRecoveringBalance = false; //this can be true even if the recovery delay has not yet been passed.  In other words, this being true does not mean the player is actively restoring balance

    #endregion

    public override void OnStartServer()
    {
        _balance = _maxBalance;

        base.OnStartServer();
    }

    [Server]
    public void LoseBalance(int balanceLost)
    {
        _balance = Math.Clamp(_balance -= balanceLost, 0, _maxBalance);
        _timeOfLastBalanceLoss = Time.time;

        if (!_isRecoveringBalance)
            StartCoroutine(RecoverBalance(_balanceRecoveredPerInterval));

        Debug.Log($"..{name} loses {balanceLost} balance and now has {_balance}");
    }

    [Server]
    IEnumerator RecoverBalance(int balanceRecovered)
    {
        _isRecoveringBalance = true;

        while (_balance < _maxBalance)
        {
            yield return new WaitForSeconds(_recoveryInterval);

            if (_timeOfLastBalanceLoss + _recoveryDelay < Time.time)
            {
                _balance += balanceRecovered;
                Math.Clamp(_balance, 0, _maxBalance);
            }
        }

        _isRecoveringBalance = false;
    }

    void UpdateUserInterface(int oldBalanceValue, int newBalanceValue)
    {
        playerUIController.SetBalanceText(newBalanceValue);
    }

}
