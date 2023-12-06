using Mirror;
using UnityEngine;

public class PlayerDuelistController : DuelistCharacterController
{

    public override void Tick()
    {
        if (isServerOnly) HandleTickOnServer();
        else if (isServer) HandleTickOnHost();
        else if (isLocalPlayer) HandleTickOnLocalClient();
        else if (isClient) HandleTickOnClient();
    }

    [Client]
    void HandleTickOnLocalClient()
    {
        if (!_latestServerState.Equals(default(StatePayload)) && (lastProcessedState.Equals(default(StatePayload)) || !_latestServerState.Equals(lastProcessedState)))
            HandleServerReconciliation();

        int bufferIndex = currentTick % BUFFER_SIZE;

        InputPayload inputPayload = new(currentTick, Time.time - lastTickEndTime);

        foreach (DuelistControllerModule module in controllerModules)
        {
            if (module is IDuelistInputRecorder inputRecorder)
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
}
