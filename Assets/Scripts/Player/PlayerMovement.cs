using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public struct InputPayload
{
    public int tick;
    public Vector3 movementInput;
    public Vector3 rotationInput;
}

public struct StatePayload
{
    public int tick;
    public Vector3 position;
    public Quaternion rotation;
}

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(NetworkTransform))] //Server Authoritative network transform propogates state changes to other clients.
[RequireComponent(typeof(NetworkAnimator))] //Same as above but with movement animation
public class PlayerMovement : NetworkBehaviour
{
    float timer;
    int currentTick;
    float minTimeBetweenServerTicks;
    const int BUFFER_SIZE = 1024;
    [SerializeField] float movementSpeed = 5f;
    [SerializeField] float acceptablePositionError = 0.001f;
    [SerializeField] NetworkTransform networkTransform;
    [SerializeField] NetworkAnimator networkAnimator;
    [SerializeField] CharacterController characterController;
    [SerializeField] Animator animator;
    static int forwardHash = Animator.StringToHash("Forward");
    static int rightHash = Animator.StringToHash("Right");

    float verticalVelocity = 0f;

    //client only
    private StatePayload[] clientStateBuffer;
    private InputPayload[] clientInputBuffer;
    StatePayload latestServerState;
    StatePayload lastProcessedState;
    Vector3 movementInput = new Vector3();
    Vector3 rotationInput = new Vector3();

    //server only
    StatePayload[] serverStateBuffer;
    Queue<InputPayload> inputQueue;

    void Start()
    {
        minTimeBetweenServerTicks = 1f / MirkwoodNetworkManager.singleton.serverTickRate;
    }

    public override void OnStartLocalPlayer()
    {
        //Network Transform is needed to interpolate & send state updates to other clients.  It is not necessary for the local player
        networkTransform.enabled = false;
        networkAnimator.enabled = false;

        clientStateBuffer = new StatePayload[BUFFER_SIZE];
        clientInputBuffer = new InputPayload[BUFFER_SIZE];
    }

    public override void OnStartServer()
    {
        serverStateBuffer = new StatePayload[BUFFER_SIZE];
        inputQueue = new Queue<InputPayload>();
    }

    void OnMove(InputValue input)
    {
        movementInput.x = input.Get<Vector2>().x;
        movementInput.z = input.Get<Vector2>().y;
    }

    void OnLook(InputValue input)
    {
        rotationInput = input.Get<Vector2>();
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
        if (!latestServerState.Equals(default(StatePayload)) &&
        (lastProcessedState.Equals(default(StatePayload)) ||
        !latestServerState.Equals(lastProcessedState)))
            HandleServerReconciliation();

        int bufferIndex = currentTick % BUFFER_SIZE;

        //Add the input payload to the input buffer
        InputPayload inputPayload = new InputPayload
        {
            tick = currentTick,
            movementInput = movementInput,
            rotationInput = rotationInput
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
            RpcOnServerMovementState(serverStateBuffer[bufferIndex]);

    }

    [ClientRpc]
    void RpcOnServerMovementState(StatePayload statePayload)
    {
        latestServerState = statePayload;
    }

    StatePayload ProcessMovement(InputPayload input)
    {

        if (!characterController.isGrounded)
            verticalVelocity += Physics.gravity.y;
        else
            verticalVelocity = Physics.gravity.y;

        Vector3 movementLocalDirection = new Vector3(input.movementInput.x, verticalVelocity, input.movementInput.z);

        characterController.Move(transform.TransformDirection(movementLocalDirection) *
                                 movementSpeed *
                                 minTimeBetweenServerTicks);
        transform.Rotate(transform.up, input.rotationInput.x);

        AnimateMovement();

        return new StatePayload()
        {
            tick = input.tick,
            position = transform.position,
            rotation = transform.rotation
        };
    }

    void AnimateMovement()
    {
        animator.SetFloat(forwardHash, transform.InverseTransformDirection(characterController.velocity).z);
        animator.SetFloat(rightHash, transform.InverseTransformDirection(characterController.velocity).x);
    }

    [Client]
    void HandleServerReconciliation()
    {
        lastProcessedState = latestServerState;

        int serverStateBufferIndex = latestServerState.tick % BUFFER_SIZE;

        float positionError = Vector3.Distance(latestServerState.position, clientStateBuffer[serverStateBufferIndex].position);

        //dont care about rotation error for right now. I don't see why we would need to reconcile this
        // float rotationError = Quaternion.Angle(latestServerState.rotation, clientStateBuffer[serverStateBufferIndex].rotation);

        if (positionError > acceptablePositionError)
        {
            Debug.Log($"..Reconciling for {positionError} position error");

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
