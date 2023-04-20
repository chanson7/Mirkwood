using Mirror;
using UnityEngine;

[RequireComponent(typeof(PredictedPlayerTransform))]
public abstract class PredictedPlayerTickProcessor : NetworkBehaviour
{

    public PredictedPlayerTransform predictedPlayerTransform;

    public virtual void Start()
    {
        predictedPlayerTransform = gameObject.GetComponent<PredictedPlayerTransform>();

        predictedPlayerTransform.RegisterPlayerTickProcessor(this);
    }

    public abstract InputPayload GatherInput(InputPayload inputPayload); //only runs on the local client
    public abstract StatePayload ProcessTick(StatePayload statePayload, InputPayload inputPayload); //runs on the local client and the server

}

public struct InputPayload
{
    public int Tick;
    public Vector3 MoveDirection;
    public bool IsWalking;
    public Vector3 LookAtDirection;
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
    Dodge,
    Attack,
    Block,
    None
}
