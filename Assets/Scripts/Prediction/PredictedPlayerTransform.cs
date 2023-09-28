using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PredictedPlayerTransform : NetworkBehaviour
{

    #region EDITOR EXPOSED FIELDS

    [SerializeField] float acceptablePositionError = 0.001f;

    [Tooltip("Each player state processor runs the same processing function once per tick on both the client and the server")]
    [SerializeField] List<PredictedTransformModule> _predictedTransformModules = new();

    #endregion

    #region FIELDS

    [SyncVar] float _serverTickMs;
    StatePayload[] _stateBuffer;
    int _currentTick;
    float _tickTimer;

    //client only
    float _lastTickEndTime = 0f;
    InputPayload[] _clientInputBuffer;
    StatePayload _latestServerState;
    StatePayload _lastProcessedState;

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

        _stateBuffer = new StatePayload[BUFFER_SIZE];
        _clientInputBuffer = new InputPayload[BUFFER_SIZE];

        base.OnStartLocalPlayer();
    }

    public override void OnStartServer()
    {
        _stateBuffer = new StatePayload[BUFFER_SIZE];
        _inputQueue = new Queue<InputPayload>();
        _serverTickMs = 1f / NetworkManager.singleton.sendRate;

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
        if (!_latestServerState.Equals(default(StatePayload)) && (_lastProcessedState.Equals(default(StatePayload)) || !_latestServerState.Equals(_lastProcessedState)))
            HandleServerReconciliation();

        int bufferIndex = _currentTick % BUFFER_SIZE;

	    InputPayload inputPayload = new(_currentTick, Time.time - _lastTickEndTime);

	    foreach (PredictedTransformModule transformModule in _predictedTransformModules)
	    	if(transformModule is IPredictedInputRecorder inputRecorder)
		    	inputRecorder.RecordInput(ref inputPayload);
	    
        _clientInputBuffer[bufferIndex] = inputPayload;
        _stateBuffer[bufferIndex] = ProcessInput(inputPayload);

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
            _stateBuffer[bufferIndex] = statePayload;
        }

        //we processed all of the input!!
        if (bufferIndex != -1)
            RpcOnServerMovementState(_stateBuffer[bufferIndex]);

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
        StatePayload processedState = input.Tick > 0 ? new(_stateBuffer[(input.Tick - 1) % BUFFER_SIZE]) : new(input.Tick, transform);

        foreach (PredictedTransformModule transformModule in _predictedTransformModules)
            if (transformModule is IPredictedStateProcessor stateProcessor)
                stateProcessor.ProcessTick(ref processedState, input);

        return processedState;
    }

    [Client]
    void HandleServerReconciliation()
    {
        _lastProcessedState = _latestServerState;

        int serverStateBufferIndex = _latestServerState.Tick % BUFFER_SIZE;

        float positionError = Vector3.Distance(_latestServerState.Position, _stateBuffer[serverStateBufferIndex].Position);

        // this is how to find the difference between the rotations i guess
        //Quaternion serverRotation = Quaternion.identity * Quaternion.Inverse(_latestServerState.Rotation);
        //Quaternion clientRotation = Quaternion.identity * Quaternion.Inverse(_clientStateBuffer[serverStateBufferIndex].Rotation);

        //Quaternion rotationError = clientRotation * Quaternion.Inverse(serverRotation);
        // Debug.Log($"euler angles magnitude{rotationError.eulerAngles}\nrotation error {rotationError}");

        if (positionError > acceptablePositionError)
        {
            Debug.Log($"..Reconciling for {positionError} position error");

            // Rewind & Replay
            transform.position = _latestServerState.Position;

            // Update buffer at index of latest server state
            _stateBuffer[serverStateBufferIndex] = _latestServerState;

            // Now re-simulate the rest of the ticks up to the current tick on the client
            int tickToProcess = _latestServerState.Tick + 1;

            while (tickToProcess < _currentTick)
            {
                int bufferIndex = tickToProcess % BUFFER_SIZE;

                // Process new movement with reconciled state
                StatePayload statePayload = ProcessInput(_clientInputBuffer[bufferIndex]);

                // Update buffer with recalculated state
                _stateBuffer[bufferIndex] = statePayload;

                tickToProcess++;
            }
        }
    }

    #endregion

    #region MONOBEHAVIOUR

    void Update()
    {
        _tickTimer += Time.deltaTime;

        while (_tickTimer >= _serverTickMs)
        {
            _tickTimer -= _serverTickMs;
            Tick();

            _lastTickEndTime = Time.time;
            _currentTick++;
        }
    }

    #endregion

}
