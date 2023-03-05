using Mirror;
using UnityEngine;

[RequireComponent(typeof(PredictedPlayerTransform))]
public abstract class PredictedPlayerTickProcessor : NetworkBehaviour
{
    private void Start() { gameObject.GetComponent<PredictedPlayerTransform>().RegisterPlayerInputProcessor(this); }

    public abstract InputPayload GatherInput(InputPayload movementPayload); //only runs on the local client
    public abstract StatePayload ProcessTick(StatePayload statePayload, InputPayload movementPayload); //runs on the local client and the server

}

public struct InputPayload
{
    public int Tick;
    public Vector3 MoveDirection;
    public bool IsSprinting;
    public Vector3 LookAtDirection;
}

public struct StatePayload
{
    public int Tick;
    public Vector3 Position;
    public Quaternion Rotation;
    public Vector3 CurrentVelocity;
}