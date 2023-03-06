using Mirror;
using UnityEngine;

[RequireComponent(typeof(PredictedPlayerTransform))]
public abstract class PredictedPlayerTickProcessor : NetworkBehaviour
{
    public virtual void Start()
    {
        gameObject.GetComponent<PredictedPlayerTransform>().RegisterPlayerTickProcessor(this);
    }

    public abstract InputPayload GatherInput(InputPayload inputPayload); //only runs on the local client
    public abstract StatePayload ProcessTick(StatePayload statePayload, InputPayload inputPayload); //runs on the local client and the server

}

public struct InputPayload
{
    public int Tick;
    public Vector3 MoveDirection;
    public bool IsSprinting;
    public Vector3 LookAtDirection;
    public AnimationPriority ActiveAnimationPriority;
}

public struct StatePayload
{
    public int Tick;
    public Vector3 Position;
    public Quaternion Rotation;
    public Vector3 CurrentVelocity;
}

public enum AnimationPriority
{
    Interrupt,
    Dodge,
    Attack,
    None
}
