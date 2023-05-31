using Mirror;
using UnityEngine;

[RequireComponent(typeof(PredictedPlayerTransform))]
public abstract class PredictedStateProcessor : NetworkBehaviour
{

    public PredictedPlayerTransform predictedPlayerTransform;

    public virtual void Start()
    {
        predictedPlayerTransform = gameObject.GetComponent<PredictedPlayerTransform>();

        predictedPlayerTransform.RegisterPlayerTickProcessor(this);
    }

    public abstract StatePayload ProcessTick(StatePayload statePayload, InputPayload inputPayload); //runs on the local client and the server

    public abstract void OnInterrupt();

}

//just like a predicted state processor, but allows the user to input things.
public abstract class PredictedPlayerInputProcessor : PredictedStateProcessor
{
    public abstract InputPayload GatherInput(InputPayload inputPayload); //only runs on the local client
}

public struct InputPayload
{
    public int Tick;
    public Vector3 MoveDirection;
    public Vector3 LookAtDirection;
    public bool IsWalking;
    public PlayerAnimationEvent ActiveAction;
}

public struct StatePayload
{
    public int Tick;
    public Vector3 Position;
    public Quaternion Rotation;
    public Vector3 CurrentVelocity;
}

public enum PlayerAnimationEvent
{
    Voluntary,
    Involuntary,
    Dodge,
    Attack,
    Block,
    None
}
