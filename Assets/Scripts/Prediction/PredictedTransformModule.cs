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

    //todo combine all buttons into single ButtonsPressed enum
    public bool AttackPressed;

}

public struct StatePayload
{
    public StatePayload(Transform transform)
    {
        Tick = 0;
        Position = transform.position;
        Rotation = transform.rotation;
        LookDirection = 0f;
        Velocity = Vector3.zero;
        LastStateChangeTick = 0;
        HitVector = Vector3.zero;
        /*_playerState*/ PlayerState = PlayerState.Balanced;
    }

    //Construct a new state based off of the previous Tick.
    public StatePayload(StatePayload previousStatePayload)
    {
        Tick = previousStatePayload.Tick + 1;
        Position = previousStatePayload.Position;
        Rotation = previousStatePayload.Rotation;
        LookDirection = previousStatePayload.LookDirection;
        Velocity = previousStatePayload.Velocity;
        LastStateChangeTick = previousStatePayload.LastStateChangeTick;
        HitVector = previousStatePayload.HitVector;
        /*_playerState*/ PlayerState = previousStatePayload.PlayerState;
    }

    //PlayerState _playerState;

    public int Tick;
    public Vector3 Position;
    public Quaternion Rotation;
    public float LookDirection;
    public Vector3 Velocity;
    public Vector3 HitVector;
    public int LastStateChangeTick;
    public PlayerState PlayerState;
    //public PlayerState PlayerState {
    //    readonly get 
    //    {
    //        return _playerState; 
    //    }
    //    set 
    //    {
    //        LastStateChangeTick = Tick;
    //        _playerState = value;
    //    } 
    //}
}

public enum PlayerState : byte
{
    Balanced = 1,
    Attack1 = 2,
    Attack2 = 4,
    Attack3 = 8,
    Hit = 16
}