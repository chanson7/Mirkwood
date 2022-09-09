using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public struct InputPayload
{
    public int tick;
    public Vector3 movementInput;
}

public struct StatePayload
{
    public int tick;
    public Vector3 position;
}

public class PlayerMovement : NetworkBehaviour
{
    float timer;
    int currentTick;
    float minTimeBetweenServerTicks;
    const int BUFFER_SIZE = 1024;
    [SerializeField] float movementSpeed = 5f;
    [SerializeField] float acceptablePositionError = 0.001f;
    [SerializeField] NetworkTransform networkTransform;

    //client only
    private StatePayload[] clientStateBuffer;
    private InputPayload[] clientInputBuffer;
    StatePayload latestServerState;
    StatePayload lastProcessedState;
    Vector3 movementInput = new Vector3();

    //server only
    StatePayload[] serverStateBuffer;
    Queue<InputPayload> inputQueue;

    void Start()
    {
        minTimeBetweenServerTicks = 1f / MirkwoodNetworkManager.singleton.serverTickRate;
    }

    public override void OnStartLocalPlayer()
    {
        Debug.Log($"..Initializing CLIENT side player movement for {this.gameObject.name}");

        //Network Transform is needed to interpolate & send state updates to other clients.  It is not necessary for the local player
        networkTransform.enabled = false;

        clientStateBuffer = new StatePayload[BUFFER_SIZE];
        clientInputBuffer = new InputPayload[BUFFER_SIZE];
    }

    public override void OnStartServer()
    {
        Debug.Log($"..Initializing SERVER side player movement for {this.gameObject.name}");

        serverStateBuffer = new StatePayload[BUFFER_SIZE];
        inputQueue = new Queue<InputPayload>();
    }

    void OnMove(InputValue input)
    {
        movementInput.x = input.Get<Vector2>().x;
        movementInput.z = input.Get<Vector2>().y;
    }

    void Update()
    {
        timer += Time.deltaTime;

        while (timer >= minTimeBetweenServerTicks)
        {
            timer -= minTimeBetweenServerTicks;

            if (isLocalPlayer)
                HandleTickOnLocalClient();
            else if (isServer)
                HandleTickOnServer();

            currentTick++;
        }
    }

    [Client]
    void HandleTickOnLocalClient()
    {

        if (!latestServerState.Equals(default(StatePayload)) &&
        (lastProcessedState.Equals(default(StatePayload)) ||
        !latestServerState.Equals(lastProcessedState)))
            HandleServerReconciliation();

        int bufferIndex = currentTick % BUFFER_SIZE;

        //Add the input payload to the input buffer
        InputPayload inputPayload = new InputPayload
        {
            tick = currentTick,
            movementInput = movementInput
        };

        clientInputBuffer[bufferIndex] = inputPayload;
        clientStateBuffer[bufferIndex] = ProcessMovement(inputPayload);

        CmdOnClientInput(inputPayload);
    }

    [Command]
    void CmdOnClientInput(InputPayload inputPayload)
    {
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

            bufferIndex = inputPayload.tick % BUFFER_SIZE;

            StatePayload statePayload = ProcessMovement(inputPayload);
            serverStateBuffer[bufferIndex] = statePayload;
        }

        //we processed all of the input!!!!
        if (bufferIndex != -1)
        {
            RpcOnServerMovementState(serverStateBuffer[bufferIndex]);
            // UpdateStateOnOtherClients();
        }

    }

    [ClientRpc]
    void RpcOnServerMovementState(StatePayload statePayload)
    {
        latestServerState = statePayload;
    }

    [ClientRpc(includeOwner = false)]
    void UpdateStateOnOtherClients()
    {
        transform.position = latestServerState.position;
    }

    StatePayload ProcessMovement(InputPayload input)
    {
        transform.position += input.movementInput * movementSpeed * minTimeBetweenServerTicks;

        return new StatePayload()
        {
            tick = input.tick,
            position = transform.position
        };
    }

    [Client]
    void HandleServerReconciliation()
    {
        lastProcessedState = latestServerState;

        int serverStateBufferIndex = latestServerState.tick % BUFFER_SIZE;
        float positionError = Vector3.Distance(latestServerState.position, clientStateBuffer[serverStateBufferIndex].position);

        if (positionError > acceptablePositionError)
        {
            Debug.Log($"..Reconciling for {positionError} error");

            // Rewind & Replay
            transform.position = latestServerState.position;

            // Update buffer at index of latest server state
            clientStateBuffer[serverStateBufferIndex] = latestServerState;

            // Now re-simulate the rest of the ticks up to the current tick on the client
            int tickToProcess = latestServerState.tick + 1;

            while (tickToProcess < currentTick)
            {
                int bufferIndex = tickToProcess % BUFFER_SIZE;

                // Process new movement with reconciled state
                StatePayload statePayload = ProcessMovement(clientInputBuffer[bufferIndex]);

                // Update buffer with recalculated state
                clientStateBuffer[bufferIndex] = statePayload;

                tickToProcess++;
            }
        }
    }

}
