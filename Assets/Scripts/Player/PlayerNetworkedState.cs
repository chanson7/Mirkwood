using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public struct InputPayload
{
    public int tick;
    public Vector3 movementInput;
    public bool isSprinting;
    public Vector3 mouseWorldPosition;
}

public struct StatePayload
{
    public int tick;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 currentVelocity;
}

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(NetworkTransform))] //Server Authoritative network transform propogates transform changes to other clients.
public class PlayerNetworkedState : NetworkBehaviour
{
    float timer;
    int currentTick;
    float minTimeBetweenServerTicks;
    const int BUFFER_SIZE = 1024;

    Vector3 currentVelocity;
    [SerializeField] float movementSpeed = 1f;
    [SerializeField] float sprintSpeedMultiplier = 5.0f;
    [SerializeField] float acceptablePositionError = 0.001f;
    [SerializeField] NetworkTransform networkTransform;
    [SerializeField] CharacterController characterController;
    [SerializeField] Animator animator;

    static int forwardHash = Animator.StringToHash("Forward");
    static int rightHash = Animator.StringToHash("Right");

    //client only
    private StatePayload[] clientStateBuffer;
    private InputPayload[] clientInputBuffer;
    StatePayload latestServerState;
    StatePayload lastProcessedState;
    Ray pointerRay;
    [SerializeField] LayerMask pointerMask; //so that the player will not look at everything the pointer ray hits
    Vector3 movementInput = new Vector3();
    bool isSprinting = false;
    Vector3 mouseWorldPosition = new Vector3();

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

    void OnSprint(InputValue input)
    {
        isSprinting = input.isPressed;
    }

    void Update()
    {
        timer += Time.deltaTime;

        while (timer >= minTimeBetweenServerTicks)
        {
            timer -= minTimeBetweenServerTicks;

            if (isLocalPlayer)
            {
                ClientRotateTowardsMouse();
                HandleTickOnLocalClient();
                AnimateMovement(currentVelocity);
            }
            else if (isServer)
            {
                HandleTickOnServer();
            }
            else if (isClient && !isLocalPlayer)
            {
                AnimateMovement(latestServerState.currentVelocity);
            }

            currentTick++;
        }
    }

    [Client]
    void ClientRotateTowardsMouse()
    {
        pointerRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray: pointerRay, layerMask: pointerMask, maxDistance: 100f, hitInfo: out RaycastHit hit))
        {
            mouseWorldPosition = hit.point;
            mouseWorldPosition.y = transform.position.y; //transform should not rotate on Y axis
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
            isSprinting = isSprinting,
            mouseWorldPosition = mouseWorldPosition
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

        //we processed all of the input!! Send it the latest server state to the owning client for reconciliation
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

        currentVelocity = input.isSprinting ? Vector3.Lerp(currentVelocity, input.movementInput * movementSpeed * sprintSpeedMultiplier, 0.2f) :
                                              Vector3.Lerp(currentVelocity, input.movementInput * movementSpeed, 0.2f);

        Vector3 movementValue = currentVelocity * minTimeBetweenServerTicks;

        characterController.Move(movementValue);

        transform.LookAt(input.mouseWorldPosition, Vector3.up);

        return new StatePayload()
        {
            tick = input.tick,
            position = transform.position,
            rotation = transform.rotation,
            currentVelocity = currentVelocity
        };
    }

    void AnimateMovement(Vector3 currentVelocity)
    {
        animator.SetFloat(forwardHash, transform.InverseTransformDirection(currentVelocity).z);
        animator.SetFloat(rightHash, transform.InverseTransformDirection(currentVelocity).x);
    }

    [Client]
    void HandleServerReconciliation()
    {
        lastProcessedState = latestServerState;

        int serverStateBufferIndex = latestServerState.tick % BUFFER_SIZE;

        float positionError = Vector3.Distance(latestServerState.position, clientStateBuffer[serverStateBufferIndex].position);

        //dont care about rotation error for right now.
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
