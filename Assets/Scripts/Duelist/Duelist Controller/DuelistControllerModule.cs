using Mirror;
using UnityEngine;

public abstract class DuelistControllerModule : NetworkBehaviour
{

    protected DuelistCharacterController duelistCharacterController;

    public virtual void Start()
    {
        duelistCharacterController = GetComponent<DuelistCharacterController>();
    }

}

public interface IDuelistInputRecorder
{
    public void RecordInput(ref InputPayload inputPayload);
}

public interface IDuelistInputProcessor
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
        HorizontalLookDirection = 0f;
        AttackPressed = false;
        DodgePressed = false;
        BlockPressed = false;
    }

    public int Tick;
    public float TickDuration;
    public float ClientTime;
    public Vector2 MoveDirection;
    public float HorizontalLookDirection;

    //todo combine all buttons into single ButtonsPressed enum
    public bool AttackPressed;
    public bool DodgePressed;
    public bool BlockPressed;

}

public struct StatePayload
{
    public StatePayload(Transform transform)
    {
        Tick = 0;
        Position = transform.position;
        Rotation = transform.rotation;
        /*_playerState*/ CombatState = CombatState.Balanced;
        LastStateChangeTick = 0;
        Balance = 100f;
        Energy = 0;
        LastEnergyRecoveryMs = 0;
        LookDirection = 0f;
        Velocity = Vector3.zero;
        effectDuration = 0f;
        effectTranslate = Vector3.zero;
    }

    //Construct a new state based off of the previous Tick.
    public StatePayload(StatePayload previousStatePayload)
    {
        Tick = previousStatePayload.Tick + 1;
        Position = previousStatePayload.Position;
        Rotation = previousStatePayload.Rotation;
        /*_duelistState*/ CombatState = previousStatePayload.CombatState;
        LastStateChangeTick = previousStatePayload.LastStateChangeTick;
        LookDirection = previousStatePayload.LookDirection;
        Velocity = previousStatePayload.Velocity;
        Balance = previousStatePayload.Balance;
        Energy = previousStatePayload.Energy;
        LastEnergyRecoveryMs = previousStatePayload.LastEnergyRecoveryMs;
        effectDuration  = previousStatePayload.effectDuration;
        effectTranslate = previousStatePayload.effectTranslate;
    }

    //PlayerState _duelistState;

    public int Tick;
    public Vector3 Position;
    public Quaternion Rotation;
    public CombatState CombatState;
    public int LastStateChangeTick;
    public float LookDirection;
    public Vector3 Velocity;
    public float Balance;
    public int Energy;
    public float LastEnergyRecoveryMs;
    public float effectDuration;
    public Vector3 effectTranslate;
    //public CombatState CombatState {
    //    readonly get 
    //    {
    //        return _combatState; 
    //    }
    //    set 
    //    {
    //        LastStateChangeTick = Tick;
    //        _combatState = value;
    //    } 
    //}
}

public enum CombatState : byte
{
    Balanced = 1,
    Attacking_Primary = 2,
    Attacking_Secondary = 4,
    Attacking_Tertiary = 8,
    Dodging = 16,
    Blocking = 32,
    Disabled = 64,
    KnockedDown = 128
}