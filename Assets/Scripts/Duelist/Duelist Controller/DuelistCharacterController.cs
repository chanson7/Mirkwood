using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Events;

[RequireComponent(typeof(NetworkIdentity))]
public abstract class DuelistCharacterController : NetworkBehaviour
{

    #region EDITOR EXPOSED FIELDS

    [Tooltip("Each module runs the same processing function once per tick on both the client and the server")]
    [SerializeField] protected List<DuelistControllerModule> controllerModules = new();

    #endregion

    #region FIELDS

    [SyncVar] protected float _serverSendInterval;
    protected StatePayload[] stateBuffer;
    protected int currentTick;
    protected float tickTimer;
    protected Queue<UnpredictedEvent> unpredictedEffectsQueue;

    //client only
    protected readonly float acceptablePositionError = 0.001f;
    protected readonly float acceptableRotationError = 0.001f;
    protected float lastTickEndTime = 0f;
    protected InputPayload[] clientInputBuffer;
    protected StatePayload _latestServerState;
    protected StatePayload lastProcessedState;

    //server only
    protected Queue<InputPayload> inputQueue;

    #endregion

    #region PROPERTIES

    public StatePayload LatestServerState { get => _latestServerState; }
    public float ServerSendInterval { get => _serverSendInterval; }
    public UnityEvent<StatePayload> EvtServerStateProcessed; //invoked on server only

    #endregion

    #region CONSTANTS

    protected const int BUFFER_SIZE = 1024;

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

    public abstract void Tick();

    [Server]
    protected virtual void HandleTickOnServer()
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

        if(gameObject.GetComponent<PlayerDuelist>()) //might be a better way to check with mirror.  Only want to enqueue on player clients
            TargetEnqueueUnpredictedEvent(effect);

        [TargetRpc]
        void TargetEnqueueUnpredictedEvent(UnpredictedEvent effect)
        {
            unpredictedEffectsQueue.Enqueue(effect);
        }
    }

    protected void HandleTickOnHost()
    {
        InputPayload inputPayload = new(currentTick, Time.time - lastTickEndTime);
        int bufferIndex = inputPayload.Tick % BUFFER_SIZE;

        foreach (DuelistControllerModule module in controllerModules)
            if (module is IDuelistInputRecorder inputRecorder)
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

    protected void HandleTickOnClient()
    {
        transform.SetPositionAndRotation(_latestServerState.Position, _latestServerState.Rotation);
    }

    protected StatePayload ProcessTick(InputPayload input)
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
        foreach (DuelistControllerModule transformModule in controllerModules)
        {
            if (transformModule is IDuelistInputProcessor inputProcessor)
            {
                inputProcessor.ProcessInput(ref state, input);
            }
        }

        state.Velocity = (state.Position - previousPosition) / input.TickDuration;

        return state;
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
}
