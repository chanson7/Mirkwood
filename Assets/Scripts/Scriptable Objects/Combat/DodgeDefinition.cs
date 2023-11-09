using UnityEngine;

[CreateAssetMenu(fileName = "Dodge Definition", menuName = "Combat/Dodge Definition", order = 2)]
public class DodgeDefinition : ScriptableObject
{
    [SerializeField] string _animationHash;

    public int AnimationHash { get { return Animator.StringToHash(_animationHash); } }

    [Header("Dodge Properties")]
    [Tooltip("The length of time an attack lasts (in seconds)")]
    [Range(0f, 5f)]
    public float DodgeDuration;

    [Tooltip("The distance a player should move forwards during the attack animation")]
    [Range(0f, 5f)]
    public float DodgeDistance;

}
