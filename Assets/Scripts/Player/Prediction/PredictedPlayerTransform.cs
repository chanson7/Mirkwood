using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(PredictedPlayerReceiveHit), typeof(NetworkIdentity), typeof(PlayerObject))]
public class PredictedPlayerTransform : NetworkBehaviour
{

    #region EDITOR EXPOSED FIELDS

    [Tooltip("Each module runs the same processing function once per tick on both the client and the server")]
    [SerializeField] List<PredictedTransformModule> predictedTransformModules = new();

    #endregion

    #region FIELDS

    [SyncVar] float _serverTickMs;
    StatePayload[] stateBuffer;
    int currentTick;
    float tickTimer;
    Queue<UnpredictedTransformEffect> unpredictedEffectsQueue;
    PlayerObject playerObject;

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
    public float ServerTickMs { get => _serverTickMs; }

    #endregion
    
    #region CONSTANTS

    const int BUFFER_SIZE = 1024;

    #endregion

    #region NETWORKBEHAVIOUR

    public override void OnStartLocalPlayer()
    {
        stateBuffer = new StatePayload[BUFFER_SIZE];
        unpredictedEffectsQueue = new Queue<UnpredictedTransformEffect>();
        clientInputBuffer = new InputPayload[BUFFER_SIZE];

        base.OnStartLocalPlayer();
    }

    public override void OnStartServer()
    {
        stateBuffer = new StatePayload[BUFFER_SIZE];
        unpredictedEffectsQueue = new Queue<UnpredictedTransformEffect>();
        inputQueue = new Queue<InputPayload>();
        _serverTickMs = 1f / NetworkManager.singleton.sendRate;

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

        foreach (PredictedTransformModule transformModule in predictedTransformModules)
            if (transformModule is IPredictedInputRecorder inputRecorder)
                inputRecorder.RecordInput(ref inputPayload);

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

        //we processed all of the input!!
        if (bufferIndex != -1)
        {
            RpcOnServerStateUpdated(stateBuffer[bufferIndex]);
        }
    }

    [Server]
    public void EnqueueUnpredictedEvent(UnpredictedTransformEffect effect)
    {
        unpredictedEffectsQueue.Enqueue(effect);

        TargetEnqueueUnpredictedEvent(effect);

        [TargetRpc]
        void TargetEnqueueUnpredictedEvent(UnpredictedTransformEffect effect)
        {
            unpredictedEffectsQueue.Enqueue(effect);
        }
    }


    void HandleTickOnHost()
    {
        InputPayload inputPayload = new(currentTick, Time.time - lastTickEndTime);
        int bufferIndex = inputPayload.Tick % BUFFER_SIZE;

        foreach (PredictedTransformModule transformModule in predictedTransformModules)
            if (transformModule is IPredictedInputRecorder inputRecorder)
                inputRecorder.RecordInput(ref inputPayload);

        stateBuffer[bufferIndex] = ProcessTick(inputPayload);

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
        Vector3 previousPosition = state.Position; //used to calculate velocity

        //apply any unpredicted effects to the state
        while (unpredictedEffectsQueue.Count > 0)
        {
            UnpredictedTransformEffect effect = unpredictedEffectsQueue.Dequeue();
            
            state.effectDuration += effect.Duration;
            state.effectTranslate += effect.Translation;
        }

        //process the player's predictable inputs
        foreach (PredictedTransformModule transformModule in predictedTransformModules)
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

        while (tickTimer >= _serverTickMs)
        {
            tickTimer -= _serverTickMs;
            Tick();
            
            lastTickEndTime = Time.time;
            currentTick++;
        }
    }

    private void Awake()
    {
        playerObject = GetComponent<PlayerObject>();
    }

    #endregion

}

public struct UnpredictedTransformEffect
{
    public Vector3 Translation;
    public float Duration;
    public bool ServerWait;
}
