using UnityEngine;

[CreateAssetMenu(fileName = "Attack Definition", menuName = "Combat/Attack Definition", order = 2)]
public class AttackDefinition : ScriptableObject
{
    [SerializeField] string _animationHash;

    public int AnimationHash { get { return Animator.StringToHash(_animationHash); } }

    [Header("Attack Properties")]
    [Tooltip("The length of time an attack lasts (in seconds)")]
    [Range(0f, 5f)]
    public float AttackDuration;

    [Tooltip("The distance a player should move forwards during the attack animation")]
    [Range(0f, 5f)]
    public float LungeDistance;

    [Tooltip("The distance a player should move forwards during the attack animation")]
    public float LungeDuration { get { return LungeEndTime - LungeStartTime; } }

    [Header("Timing Checkpoints")]
    [Range(0f, 1f)]
    [Tooltip("The time at which the hit should be applied, as a percentage of the attack duration")]
    public float HitApplicationTime = 0.5f;

    [Range(0f, 1f)]
    [Tooltip("The time at which the lunge movement begins, as a percentage of the attack duration")]
    public float LungeStartTime = 0.5f;

    [Range(0f, 1f)]
    [Tooltip("The time at which the lunge movement ends, as a percentage of the attack duration")]
    public float LungeEndTime = 0.5f;

    [Header("On Hit Effects")]
    [Tooltip("Multiplier for the distance that the player is knocked back")]
    [Range(0f, 5f)]
    public float KnockbackMultiplier;

    [Tooltip("The duration of the stun effect")]
    [Range(0f, 5f)]
    public float StunDuration;
}
