using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;
public class PlayerAnimation : NetworkBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Animator rootMotionAnimator;
    [SerializeField] NetworkAnimator networkAnimator;
    Vector3 rootMotionDeltaPosition;
    public int triggeredAnimationHash = -1;

    //animation hashes that should probably be moved to a scriptable object at some point maybe idk
    static int attackHash = Animator.StringToHash("Attack");

    void OnAttack(InputValue input) { triggeredAnimationHash = attackHash; }

    //when the triggered hash is retrieved, reset it to -1
    public int getTriggeredAnimationHash()
    {
        int hash = triggeredAnimationHash;
        triggeredAnimationHash = -1;

        return hash;
    }

    public void SetRootMotionAnimationTrigger(int animationHash)
    {

    }

    public StatePayload ProcessRootMotion(StatePayload statePayload, Vector3 deltaRootMotion)
    {
        statePayload.Position += deltaRootMotion;

        return statePayload;
    }

    private void FixedUpdate()
    {
        rootMotionDeltaPosition += rootMotionAnimator.deltaPosition;
    }

}
