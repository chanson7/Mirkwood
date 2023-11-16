using UnityEngine;
using System.Collections;
using Mirror;
using System;

public class PredictedPlayerBalance : PredictionModule, IPredictedInputProcessor
{

    #region EDITOR EXPOSED FIELDS

    [Header("Balance Configuration")]
    [Tooltip("The base rate at which balance is recovered")]
    [SerializeField] float baseBalanceRecoveryRate = 1f;

    [Header("")]
    [SerializeField] ScriptableEvent EvtPlayerKnockedDown;

    #endregion

    public void ProcessInput(ref StatePayload statePayload, InputPayload inputPayload)
    {
        if(statePayload.Balance <= 0f && !statePayload.PlayerState.Equals(PlayerState.KnockedDown))
        {
            statePayload.PlayerState = PlayerState.KnockedDown;
            statePayload.LastStateChangeTick = statePayload.Tick;
            statePayload.Balance = 0f;

            PlayerKnockedDown();

            return;
        }

        if(statePayload.Balance <= 100f)
        {
            statePayload.Balance += baseBalanceRecoveryRate * inputPayload.TickDuration;
            statePayload.Balance = Mathf.Min(100f, statePayload.Balance + baseBalanceRecoveryRate * inputPayload.TickDuration);
        }
    }

    void PlayerKnockedDown()
    {
        Debug.Log($"{name} has been knocked down!");

        EvtPlayerKnockedDown.Raise();
    }
}
