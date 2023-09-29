using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PredictedPlayerTransform : NetworkBehaviour
{

    #region EDITOR EXPOSED FIELDS

    [SerializeField] float acceptablePositionError = 0.001f;
    [SerializeField] float acceptableRotationError = 0.001f;

    [Tooltip("Each module runs the same processing function once per tick on both the client and the server")]
    [SerializeField] List<PredictedTransformModule> predictedTransformModules = new();

    #endregion

    #region FIELDS

    [SyncVar] float serverTickMs;
    StatePayload[] stateBuffer;
    int currentTick;
    float tickTimer;

    //client only
    float lastTickEndTime = 0f;
    InputPayload[] clientInputBuffer;
    StatePayload _latestServerState;
    StatePayload lastProcessedState;

    //server only
    Queue<InputPayload> _inputQueue;

    #endregion

    #region PROPERTIES
    public StatePayload LatestServerState { get { return _latestServerState; } }

    #endregion

    #region CONSTANTS

    const int BUFFER_SIZE = 1024;

    #endregion

    #region NETWORKBEHAVIOUR

    public override void OnStartLocalPlayer()
    {

        stateBuffer = new StatePayload[BUFFER_SIZE];
        clientInputBuffer = new InputPayload[BUFFER_SIZE];

        base.OnStartLocalPlayer();
    }

    public override void OnStartServer()
    {
        stateBuffer = new StatePayload[BUFFER_SIZE];
        _inputQueue = new Queue<InputPayload>();
        serverTickMs = 1f / NetworkManager.singleton.sendRate;

        base.OnStartServer();
    }

    #endregion

    #region METHODS

    public void Tick()
    {
        if (isLocalPlayer)
            HandleTickOnLocalClient();
        else if (isServer)
            HandleTickOnServer();
        else if (isClient && !isLocalPlayer)
            HandleTickOnOtherClient();
    }

    [Client]
    void HandleTickOnLocalClient()
    {
        if (!_latestServerState.Equals(default(StatePayload)) && (lastProcessedState.Equals(default(StatePayload)) || !_latestServerState.Equals(lastProcessedState)))
            HandleServerReconciliation();

        int bufferIndex = currentTick % BUFFER_SIZE;

	    InputPayload inputPayload = new(currentTick, Time.time - lastTickEndTime);

	    foreach (PredictedTransformModule transformModule in predictedTransformModules)
	    	if(transformModule is IPredictedInputRecorder inputRecorder)
		    	inputRecorder.RecordInput(ref inputPayload);
	    
        clientInputBuffer[bufferIndex] = inputPayload;
        stateBuffer[bufferIndex] = ProcessInput(inputPayload);

        CmdOnClientInput(inputPayload);
    }

    [Command]
    void CmdOnClientInput(InputPayload inputPayload)
    {
        //TODO a client can just send any frequency of inputs to speed hack. this is bad
        _inputQueue.Enqueue(inputPayload);
    }

    [Server]
    void HandleTickOnServer()
    {
        int bufferIndex = -1;

        //server has some movements to process
        while (_inputQueue.Count > 0)
        {
            InputPayload inputPayload = _inputQueue.Dequeue();

            bufferIndex = inputPayload.Tick % BUFFER_SIZE;

            StatePayload statePayload = ProcessInput(inputPayload);
            stateBuffer[bufferIndex] = statePayload;
        }

        //we processed all of the input!!
        if (bufferIndex != -1)
            RpcOnServerMovementState(stateBuffer[bufferIndex]);

    }

    [ClientRpc]
    void RpcOnServerMovementState(StatePayload statePayload)
    {
        _latestServerState = statePayload;
    }

    void HandleTickOnOtherClient()
    {
        transform.SetPositionAndRotation(_latestServerState.Position, _latestServerState.Rotation);
    }

    StatePayload ProcessInput(InputPayload input)
    {
        //if we're not in Tick 0, construct a state payload using the last state payload from the buffer
        StatePayload processedState = input.Tick > 0 ? new(stateBuffer[(input.Tick - 1) % BUFFER_SIZE]) : 
                                                       new(input.Tick, transform);

        foreach (PredictedTransformModule transformModule in predictedTransformModules)
            if (transformModule is IPredictedStateProcessor stateProcessor)
                stateProcessor.ProcessTick(ref processedState, input);

        return processedState;
    }

    [Client]
    void HandleServerReconciliation()
    {
        lastProcessedState = _latestServerState;

        int serverStateBufferIndex = _latestServerState.Tick % BUFFER_SIZE;
        float positionError = Vector3.Distance(_latestServerState.Position, stateBuffer[serverStateBufferIndex].Position);
        float rotationError = (_latestServerState.Rotation * Quaternion.Inverse(stateBuffer[serverStateBufferIndex].Rotation)).eulerAngles.magnitude;

        if (positionError > acceptablePositionError)
        {
            Debug.Log($"Reconciling for {positionError} position error");

            //reset position
            transform.position = _latestServerState.Position;

            //rewind and replay
            ReconcileState(serverStateBufferIndex);
        }

        if (rotationError > acceptableRotationError)
        {
            Debug.Log($"Reconciling for {rotationError} rotation error");

            //reset rotation
            transform.rotation = _latestServerState.Rotation;

            //rewind and replay
            ReconcileState(serverStateBufferIndex);
        }

        void ReconcileState(int serverStateBufferIndex)
        {
            // Update buffer at index of latest server state
            stateBuffer[serverStateBufferIndex] = _latestServerState;

            // Now re-simulate the rest of the ticks up to the current tick on the client
            int tickToProcess = _latestServerState.Tick + 1;

            while (tickToProcess < currentTick)
            {
                int bufferIndex = tickToProcess % BUFFER_SIZE;

                // Process new movement with reconciled state
                StatePayload statePayload = ProcessInput(clientInputBuffer[bufferIndex]);

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

        while (tickTimer >= serverTickMs)
        {
            tickTimer -= serverTickMs;
            Tick();

            lastTickEndTime = Time.time;
            currentTick++;
        }
    }

    #endregion

}
