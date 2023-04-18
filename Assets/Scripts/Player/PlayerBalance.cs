using UnityEngine;
using System.Collections;
using Mirror;
using System;

public class PlayerBalance : NetworkBehaviour
{

    uint maxBalance = 100;
    [SerializeField] PlayerInterface playerInterface;

    [SyncVar(hook = nameof(UpdateUserInterface))][SerializeField] uint balance = 100;
    bool isRecoveringBalance = false;

    [Tooltip("Time in seconds for the player to recover balance")]
    [SerializeField] float recoveryIntervalSeconds = 4f;
    [Tooltip("Amount of balance recovered after each recovery interval")]
    [SerializeField] uint balanceRecoveredPerInterval = 1;

    public override void OnStartServer()
    {
        balance = maxBalance;

        base.OnStartServer();
    }

    [Server]
    public bool SpendBalance(uint balanceSpent)
    {
        if (balance < balanceSpent)
        {
            Debug.Log($"..{this.name} does not have enough balance");
            return false;
        }
        else
        {
            balance -= balanceSpent;
            Math.Clamp(balance, 0, maxBalance);

            if (!isRecoveringBalance)
                StartCoroutine(RecoverBalance(balanceRecoveredPerInterval));

            Debug.Log($"..{this.name} spends {balanceSpent} balance and now has {balance}");

            return true;
        }

    }

    [Server]
    public void LoseBalance(uint balanceLost)
    {
        balance -= balanceLost;

        Math.Clamp(balance, 0, maxBalance);

        if (!isRecoveringBalance)
            StartCoroutine(RecoverBalance(balanceRecoveredPerInterval));

        Debug.Log($"..{this.name} loses {balanceLost} balance and now has {balance}");
    }

    [Server]
    IEnumerator RecoverBalance(uint balanceRecovered)
    {
        isRecoveringBalance = true;

        while (balance < maxBalance)
        {
            yield return new WaitForSeconds(recoveryIntervalSeconds);
            balance += balanceRecovered;
            Math.Clamp(balance, 0, maxBalance);
        }
        isRecoveringBalance = false;
    }

    void UpdateUserInterface(uint oldBalanceValue, uint newBalanceValue)
    {
        playerInterface.SetBalanceText(newBalanceValue);
    }

}
