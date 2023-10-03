using System.Collections.Generic;
using UnityEngine;

public class MeleeHitZone : MonoBehaviour
{

    #region EDITOR EXPOSED FIELDS

    [Header("Attack Settings")]
    [SerializeField]
    [Tooltip("The length of time an attack lasts (in seconds)")]
    float _attackDuration = 1f;

    [SerializeField]
    [Tooltip("The distance a player should move forwards during the attack animation")]
    float _lungeDistance = 1f;

    [SerializeField]
    [Range(0f, 1f)]
    [Tooltip("The tick at which damage should be applied, as a percentage of the attack duration")]
    float _damageApplicationPoint = 0.5f;

    [SerializeField] Collider characterControllerCollider;
    [SerializeField] List<Damageable> _damageableObjectsInRange = new();

    #endregion

    #region PROPERTIES

    public List<Damageable> DamageableObjectsInRange { get => _damageableObjectsInRange; }
    public float AttackDuration { get { return _attackDuration; } }
    public float LungeDistance { get { return _lungeDistance; } }
    public float DamageApplicationPoint { get {  return _damageApplicationPoint; } }

    #endregion

    private void Start()
    {
        Physics.IgnoreCollision(GetComponent<Collider>(), characterControllerCollider, true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<Damageable>())
            DamageableObjectsInRange.Add(other.gameObject.GetComponent<Damageable>());
    }

    private void OnTriggerExit(Collider other)
    {
        if (DamageableObjectsInRange.Contains(other.gameObject.GetComponent<Damageable>()))
        {
            DamageableObjectsInRange.Remove(other.gameObject.GetComponent<Damageable>());
        }
    }

}
