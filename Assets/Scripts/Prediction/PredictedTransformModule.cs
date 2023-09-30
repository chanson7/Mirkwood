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
    public InputPayload(int tick, float tickDuration)
    {
        Tick = tick;
        TickDuration = tickDuration;
        ClientTime = Time.time;
        MoveDirection = Vector2.zero;
        LookAtDirection = Vector2.zero;
        AttackPressed = false;
    }

    public int Tick;
    public float TickDuration;
    public float ClientTime;
    public Vector2 MoveDirection;
    public Vector2 LookAtDirection;
    public bool AttackPressed; 
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
        PlayerState = PlayerState.Balanced;
    }

    //Construct a new state based off of the previous Tick.
    public StatePayload(StatePayload previousStatePayload)
    {
        Tick = previousStatePayload.Tick + 1;
        Position = previousStatePayload.Position;
        Rotation = previousStatePayload.Rotation;
        LookDirection = previousStatePayload.LookDirection;
        Velocity = previousStatePayload.Velocity;
        PlayerState = previousStatePayload.PlayerState;
    }

    public int Tick;
    public Vector3 Position;
    public Quaternion Rotation;
    public float LookDirection;
    public Vector3 Velocity;
    public PlayerState PlayerState;
}

public enum PlayerState
{
    Balanced,
    Attacking
}