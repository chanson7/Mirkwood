using UnityEngine;

[CreateAssetMenu(fileName = "Block Definition", menuName = "Combat/Block Definition", order = 3)]
public class BlockDefinition : ScriptableObject
{
    [SerializeField] string _animationHash;

    public int AnimationHash { get { return Animator.StringToHash(_animationHash); } }

    [Header("Dodge Properties")]
    [Tooltip("The length of time an attack lasts (in seconds)")]
    [Range(0f, 5f)]
    public float BlockDuration;

}
