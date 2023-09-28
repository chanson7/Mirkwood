using Mirror;
using UnityEngine;

[RequireComponent(typeof(PredictedPlayerTransform))]
public abstract class PredictedTransformModule : NetworkBehaviour
{

    protected PredictedPlayerTransform predictedPlayerTransform;

    public virtual void Start()
    {
        predictedPlayerTransform = GetComponent<PredictedPlayerTransform>();
    }

}
public interface IPredictedInputRecorder
{
    public void RecordInput(ref InputPayload inputPayload);
}

public interface IPredictedStateProcessor
{
    public abstract void ProcessTick(ref StatePayload statePayload, InputPayload inputPayload);
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
    public StatePayload(int tick, Transform transform)
    {
        Tick = tick;
        Position = transform.position;
        Rotation = transform.rotation;
        LookDirection = 0f;
        Velocity = Vector3.zero;
    }

    public StatePayload(StatePayload previousStatePayload)
    {
        Tick = previousStatePayload.Tick + 1;
        Position = previousStatePayload.Position;
        Rotation = previousStatePayload.Rotation;
        LookDirection = previousStatePayload.LookDirection;
        Velocity = previousStatePayload.Velocity;
    }

    public int Tick;
    public Vector3 Position;
    public Quaternion Rotation;
    public float LookDirection;
    public Vector3 Velocity;
}