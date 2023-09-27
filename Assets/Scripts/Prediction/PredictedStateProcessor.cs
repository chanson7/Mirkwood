using Mirror;
using UnityEngine;

[RequireComponent(typeof(PredictedPlayerTransform))]
public abstract class PredictedStateProcessor : NetworkBehaviour
{

    protected PredictedPlayerTransform predictedPlayerTransform;

    public virtual void Start()
    {
        predictedPlayerTransform = GetComponent<PredictedPlayerTransform>();
        predictedPlayerTransform.RegisterPlayerTickProcessor(this);
    }

    public abstract void ProcessTick(ref StatePayload statePayload, InputPayload inputPayload); //runs on the local client and the server

}

public interface IPredictedInputProcessor
{
    public void GatherInput(ref InputPayload inputPayload);
}

public struct InputPayload
{
    public InputPayload(int tick, float tickTime)
    {
        Tick = tick;
        TickTime = tickTime;
        MoveDirection = Vector3.zero;
        LookAtDirection = Vector2.zero;
    }

    public int Tick;
    public float TickTime;
    public Vector3 MoveDirection;
    public Vector2 LookAtDirection;
}

public struct StatePayload
{
    public StatePayload(int tick)
    {
        Tick = tick;
        Position = Vector3.zero;
        Rotation = Quaternion.identity;
        LookDirection = 0f;
        CurrentVelocity = Vector3.zero;
    }

    public int Tick;
    public Vector3 Position;
    public Quaternion Rotation;
    public float LookDirection;
    public Vector3 CurrentVelocity;
}