using System.Collections.Generic;
using UnityEngine;
using Mirror;

#region structs

public struct InputPayload
{
    public int Tick;
    public Vector3 MovementInput;
    public bool IsSprinting;
    public Vector3 MouseWorldPosition;
}

//for events where movement/rotation of the object span multiple frames like a knockback or attack animation. 
//these should override the InputPayload during the same tick.
public struct AnimationMotionPayload
{
    public int Tick;
    public Priority priority; //Will interrupt an active movement with lower priority. For example, a player starts an attack animation that moves them over 10 frames, but 3 frames in they are knocked back.  The knock back should have higher priority.
    public float duration;
}

public enum Priority
{
    Interrupt,
    Attack
}

public struct StatePayload
{
    public int Tick;
    public Vector3 Position;
    public Quaternion Rotation;
    public Vector3 CurrentVelocity;
}

#endregion

[RequireComponent(typeof(NetworkTransform))] //Server Authoritative network transform propogates transform changes to other clients.
public class PredictedTransform : NetworkBehaviour
{

    #region serialized
    [SerializeField] NetworkTransform networkTransform;

    #endregion

    #region public
    [SyncVar] public float minTimeBetweenServerTicks;

    #endregion

    #region private 
    List<PredictedTickProcessor> tickProcessors = new List<PredictedTickProcessor>();
    float timer;
    int currentTick;

    #endregion

    #region constants
    const int BUFFER_SIZE = 1024;

    #endregion

    //Processing Components

    #region client only
    private StatePayload[] clientStateBuffer;
    private InputPayload[] clientInputBuffer;
    [SerializeField] float acceptablePositionError = 0.001f;
    public StatePayload latestServerState;
    StatePayload lastProcessedState;

    #endregion

    #region server only
    StatePayload[] serverStateBuffer;
    Queue<InputPayload> inputQueue;
    AnimationMotionPayload activeAnimationMotion; //an animation-triggered motion that might move a predicted object over several frames

    #endregion

    public override void OnStartLocalPlayer()
    {
        //Network Transform is needed to interpolate & send state updates to other clients.  It is not necessary for the local player
        networkTransform.enabled = false;

        clientStateBuffer = new StatePayload[BUFFER_SIZE];
        clientInputBuffer = new InputPayload[BUFFER_SIZE];

        base.OnStartLocalPlayer();
    }

    public override void OnStartServer()
    {

        minTimeBetweenServerTicks = 1f / MirkwoodNetworkManager.singleton.serverTickRate;
        serverStateBuffer = new StatePayload[BUFFER_SIZE];
        inputQueue = new Queue<InputPayload>();
        activeAnimationMotion = new AnimationMotionPayload();

        base.OnStartServer();
    }

    public void RegisterTickProcessor(PredictedTickProcessor tickProcessor)
    {
        tickProcessors.Add(tickProcessor);
    }

    void Update()
    {
        timer += Time.deltaTime;

        while (timer >= minTimeBetweenServerTicks)
        {
            timer -= minTimeBetweenServerTicks;

            if (isLocalPlayer)
            {
                HandleTickOnLocalClient();
            }
            else if (isServer)
            {
                HandleTickOnServer();
            }

            currentTick++;
        }
    }

    [Client]
    void HandleTickOnLocalClient()
    {
        if (!latestServerState.Equals(default(StatePayload)) && (lastProcessedState.Equals(default(StatePayload)) || !latestServerState.Equals(lastProcessedState)))
            HandleServerReconciliation();

        int bufferIndex = currentTick % BUFFER_SIZE;

        //Add the input payload to the input buffer
        InputPayload inputPayload = new InputPayload { Tick = currentTick };

        foreach (PredictedTickProcessor tickProcessor in tickProcessors)
            inputPayload = tickProcessor.GatherInput(inputPayload);

        clientInputBuffer[bufferIndex] = inputPayload;
        clientStateBuffer[bufferIndex] = ProcessInput(inputPayload);

        CmdOnClientInput(inputPayload);
    }

    [Command]
    void CmdOnClientInput(InputPayload inputPayload)
    {
        //the player can just send any frequency of inputs to speed hack. this is bad and should be fixed by somebody
        inputQueue.Enqueue(inputPayload);
    }

    [Server]
    void HandleTickOnServer()
    {
        int bufferIndex = -1;

        //server has some inputs to process
        while (inputQueue.Count > 0)
        {
            InputPayload inputPayload = inputQueue.Dequeue();

            bufferIndex = inputPayload.Tick % BUFFER_SIZE;

            StatePayload statePayload = ProcessInput(inputPayload);
            serverStateBuffer[bufferIndex] = statePayload;
        }

        //we processed all of the input!!
        if (bufferIndex != -1)
            TargetOnServerMovementState(serverStateBuffer[bufferIndex]);

    }

    [TargetRpc]
    void TargetOnServerMovementState(StatePayload statePayload)
    {
        latestServerState = statePayload;
    }

    StatePayload ProcessInput(InputPayload input)
    {
        StatePayload processedState = new StatePayload();

        processedState.Tick = input.Tick;

        foreach (PredictedTickProcessor tickProcessor in tickProcessors)
            processedState = tickProcessor.ProcessTick(processedState, input);

        return processedState;
    }

    [Client]
    void HandleServerReconciliation()
    {
        lastProcessedState = latestServerState;

        int serverStateBufferIndex = latestServerState.Tick % BUFFER_SIZE;

        float positionError = Vector3.Distance(latestServerState.Position, clientStateBuffer[serverStateBufferIndex].Position);

        //this is how to find the difference between the rotations i guess
        // Quaternion serverRotation = Quaternion.identity * Quaternion.Inverse(latestServerState.Rotation);
        // Quaternion clientRotation = Quaternion.identity * Quaternion.Inverse(clientStateBuffer[serverStateBufferIndex].Rotation);

        // Quaternion rotationError = clientRotation * Quaternion.Inverse(serverRotation);
        // Debug.Log($"euler angles magnitude{rotationError.eulerAngles}\nrotation error {rotationError}");

        if (positionError > acceptablePositionError)
        {
            Debug.Log($"..Reconciling for {positionError} position error");

            // Rewind & Replay
            transform.position = latestServerState.Position;

            // Update buffer at index of latest server state
            clientStateBuffer[serverStateBufferIndex] = latestServerState;

            // Now re-simulate the rest of the ticks up to the current tick on the client
            int tickToProcess = latestServerState.Tick + 1;

            while (tickToProcess < currentTick)
            {
                int bufferIndex = tickToProcess % BUFFER_SIZE;

                // Process new movement with reconciled state
                StatePayload statePayload = ProcessInput(clientInputBuffer[bufferIndex]);

                // Update buffer with recalculated state
                clientStateBuffer[bufferIndex] = statePayload;

                tickToProcess++;
            }
        }
    }

}
