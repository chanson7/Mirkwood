using Mirror;
using UnityEngine;

[RequireComponent(typeof(PredictedPlayerTransform))]
public abstract class PredictedTickProcessor : NetworkBehaviour
{
    public abstract StatePayload ProcessTick(StatePayload statePayload, InputPayload movementPayload); //runs on the local client and the server

}

public abstract class PredictedPlayerInputProcessor : PredictedTickProcessor
{
    private void Start() { gameObject.GetComponent<PredictedPlayerTransform>().RegisterPlayerInputProcessor(this); }

    public abstract InputPayload GatherClientInput(InputPayload movementPayload); //only runs on the local client, converts client input into a movement payload

}

public struct InputPayload
{
    public int Tick;
    public Vector3 MoveDirection;
    public float MovementSpeedMultiplier;
    public Vector3 LookAtDirection;
}

public struct StatePayload
{
    public int Tick;
    public Vector3 Position;
    public Quaternion Rotation;
    public Vector3 CurrentVelocity;
}