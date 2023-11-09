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

public interface IPredictedInputProcessor
{
    public abstract void ProcessInput(ref StatePayload statePayload, InputPayload inputPayload);
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
        DodgePressed = false;
    }

    public int Tick;
    public float TickDuration;
    public float ClientTime;
    public Vector2 MoveDirection;
    public Vector2 LookAtDirection;

    //todo combine all buttons into single ButtonsPressed enum
    public bool AttackPressed;
    public bool DodgePressed;

}

public struct StatePayload
{
    public StatePayload(Transform transform)
    {
        Tick = 0;
        Position = transform.position;
        Rotation = transform.rotation;
        /*_playerState*/ PlayerState = PlayerState.Balanced;
        LastStateChangeTick = 0;
        LookDirection = 0f;
        Velocity = Vector3.zero;
        effectDisable = 0f;
        effectTranslate = Vector3.zero;
    }

    //Construct a new state based off of the previous Tick.
    public StatePayload(StatePayload previousStatePayload)
    {
        Tick = previousStatePayload.Tick + 1;
        Position = previousStatePayload.Position;
        Rotation = previousStatePayload.Rotation;
        /*_playerState*/ PlayerState = previousStatePayload.PlayerState;
        LastStateChangeTick = previousStatePayload.LastStateChangeTick;
        LookDirection = previousStatePayload.LookDirection;
        Velocity = previousStatePayload.Velocity;
        effectDisable  = previousStatePayload.effectDisable;
        effectTranslate = previousStatePayload.effectTranslate;
    }

    //PlayerState _playerState;

    public int Tick;
    public Vector3 Position;
    public Quaternion Rotation;
    public PlayerState PlayerState;
    public int LastStateChangeTick;
    public float LookDirection;
    public Vector3 Velocity;
    public float effectDisable;
    public Vector3 effectTranslate;
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
    Attacking_Primary = 2,
    Attacking_Secondary = 4,
    Attacking_Tertiary = 8,
    Dodging = 16,
    Disabled = 32
}