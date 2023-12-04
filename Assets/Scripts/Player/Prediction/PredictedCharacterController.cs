﻿using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Events;

[RequireComponent(typeof(NetworkIdentity))]
public class PredictedCharacterController : NetworkBehaviour
{

    #region EDITOR EXPOSED FIELDS

    [Tooltip("Each module runs the same processing function once per tick on both the client and the server")]
    [SerializeField] List<PredictionModule> predictionModules = new();

    #endregion

    #region FIELDS

    [SyncVar] float _serverSendInterval;
    StatePayload[] stateBuffer;
    int currentTick;
    float tickTimer;
    Queue<UnpredictedEvent> unpredictedEffectsQueue;

    //client only
    readonly float acceptablePositionError = 0.001f;
    readonly float acceptableRotationError = 0.001f;
    float lastTickEndTime = 0f;
    InputPayload[] clientInputBuffer;
    StatePayload _latestServerState;
    StatePayload lastProcessedState;

    //server only
    Queue<InputPayload> inputQueue;

    #endregion

    #region PROPERTIES

    public StatePayload LatestServerState { get => _latestServerState; }
    public float ServerSendInterval { get => _serverSendInterval; }
    public UnityEvent<StatePayload> EvtServerStateProcessed; //invoked on server only

    #endregion

    #region CONSTANTS

    const int BUFFER_SIZE = 1024;

    #endregion

    #region NETWORKBEHAVIOUR

    public override void OnStartLocalPlayer()
    {
        stateBuffer = new StatePayload[BUFFER_SIZE];
        unpredictedEffectsQueue = new Queue<UnpredictedEvent>();
        clientInputBuffer = new InputPayload[BUFFER_SIZE];

        base.OnStartLocalPlayer();
    }

    public override void OnStartServer()
    {
        stateBuffer = new StatePayload[BUFFER_SIZE];
        unpredictedEffectsQueue = new Queue<UnpredictedEvent>();
        inputQueue = new Queue<InputPayload>();
        _serverSendInterval = 1f / NetworkManager.singleton.sendRate;

        base.OnStartServer();
    }

    #endregion

    #region METHODS

    public void Tick()
    {
        if (isLocalPlayer)
            if(isServer) HandleTickOnHost();            //host
            else HandleTickOnLocalClient();             //local client
        else if (isServer) HandleTickOnServer();        //server
        else if (isClient) HandleTickOnOtherClient();   //other client
    }

    [Client]
    void HandleTickOnLocalClient()
    {
        if (!_latestServerState.Equals(default(StatePayload)) && (lastProcessedState.Equals(default(StatePayload)) || !_latestServerState.Equals(lastProcessedState)))
            HandleServerReconciliation();

        int bufferIndex = currentTick % BUFFER_SIZE;

        InputPayload inputPayload = new(currentTick, Time.time - lastTickEndTime);

        foreach (PredictionModule transformModule in predictionModules)
        {
            if (transformModule is IPredictedInputRecorder inputRecorder)
            {
                inputRecorder.RecordInput(ref inputPayload);
            }
        }

        clientInputBuffer[bufferIndex] = inputPayload;
        stateBuffer[bufferIndex] = ProcessTick(inputPayload);

        CmdOnClientInput(inputPayload);
    }

    [Command]
    void CmdOnClientInput(InputPayload inputPayload)
    {
        //TODO a client can just send any frequency of inputs to speed hack. this is bad
        inputQueue.Enqueue(inputPayload);
    }

    [Server]
    void HandleTickOnServer()
    {
        int bufferIndex = -1;

        //server has some input to process
        while (inputQueue.Count > 0)
        {
            InputPayload inputPayload = inputQueue.Dequeue();

            bufferIndex = inputPayload.Tick % BUFFER_SIZE;

            StatePayload statePayload = ProcessTick(inputPayload);
            stateBuffer[bufferIndex] = statePayload;
        }

        //No input left, send the new state back to the player
        if (bufferIndex != -1)
        {
            RpcOnServerStateUpdated(stateBuffer[bufferIndex]);
        }
    }

    [Server]
    public void EnqueueUnpredictedEvent(UnpredictedEvent effect)
    {
        unpredictedEffectsQueue.Enqueue(effect);

        TargetEnqueueUnpredictedEvent(effect);

        [TargetRpc]
        void TargetEnqueueUnpredictedEvent(UnpredictedEvent effect)
        {
            unpredictedEffectsQueue.Enqueue(effect);
        }
    }
    
    void HandleTickOnHost()
    {
        InputPayload inputPayload = new(currentTick, Time.time - lastTickEndTime);
        int bufferIndex = inputPayload.Tick % BUFFER_SIZE;

        foreach (PredictionModule predictionModule in predictionModules)
            if (predictionModule is IPredictedInputRecorder inputRecorder)
                inputRecorder.RecordInput(ref inputPayload);

        stateBuffer[bufferIndex] = ProcessTick(inputPayload);
        
        EvtServerStateProcessed.Invoke(stateBuffer[bufferIndex]);

        RpcOnServerStateUpdated(stateBuffer[bufferIndex]);
    }

    [ClientRpc]
    void RpcOnServerStateUpdated(StatePayload statePayload)
    {
        _latestServerState = statePayload;
    }

    void HandleTickOnOtherClient()
    {
        transform.SetPositionAndRotation(_latestServerState.Position, _latestServerState.Rotation);
    }

    StatePayload ProcessTick(InputPayload input)
    {
        //if we're not in Tick 0, construct a state payload using the last state payload from the buffer
        StatePayload state = input.Tick > 0 ? new(stateBuffer[(input.Tick - 1) % BUFFER_SIZE]) : 
                                              new(transform);
        Vector3 previousPosition = state.Position;

        //apply any unpredicted effects to the state
        while (unpredictedEffectsQueue.Count > 0)
        {
            UnpredictedEvent effect = unpredictedEffectsQueue.Dequeue();
            
            state.effectDuration += effect.Duration;
            state.effectTranslate += effect.Translation;
        }

        //process the player's predictable inputs
        foreach (PredictionModule transformModule in predictionModules)
        {
            if (transformModule is IPredictedInputProcessor inputProcessor)
            {
                inputProcessor.ProcessInput(ref state, input);
            }
        }

        state.Velocity = (state.Position - previousPosition) / input.TickDuration;

        return state;
    }

    [Client]
    void HandleServerReconciliation()
    {
        lastProcessedState = _latestServerState;

        int serverStateBufferIndex = _latestServerState.Tick % BUFFER_SIZE;
        float positionError = Vector3.Distance(_latestServerState.Position, stateBuffer[serverStateBufferIndex].Position);
        float rotationError = (_latestServerState.Rotation * Quaternion.Inverse(stateBuffer[serverStateBufferIndex].Rotation)).eulerAngles.magnitude;

        if (positionError > acceptablePositionError || rotationError > acceptableRotationError)
        {
            Debug.Log($"Reconciling for {positionError} position error and/or {rotationError} rotation error");

            //reset position and rotation
            transform.SetPositionAndRotation(_latestServerState.Position, _latestServerState.Rotation);

            // Update buffer at index of latest server state
            stateBuffer[serverStateBufferIndex] = _latestServerState;

            // Now re-simulate the rest of the ticks up to the current tick on the client
            int tickToProcess = _latestServerState.Tick + 1;

            while (tickToProcess < currentTick)
            {
                int bufferIndex = tickToProcess % BUFFER_SIZE;

                // Process new movement with reconciled state
                StatePayload statePayload = ProcessTick(clientInputBuffer[bufferIndex]);

                // Update buffer with recalculated state
                stateBuffer[bufferIndex] = statePayload;

                tickToProcess++;
            }
        }
    }

    #endregion

    #region MONOBEHAVIOUR

    void Update()
    {
        tickTimer += Time.deltaTime;

        while (tickTimer >= _serverSendInterval)
        {
            tickTimer -= _serverSendInterval;
            Tick();
            
            lastTickEndTime = Time.time;
            currentTick++;
        }
    }

    #endregion

}

public struct UnpredictedEvent
{
    public Vector3 Translation;
    public float Duration;
    public bool ServerWait;
}
