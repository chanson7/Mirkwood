using System.Collections.Generic;
using UnityEngine;

public class MeleeCollider : MonoBehaviour
{
    [SerializeField] Collider characterControllerCollider;
    
    #region EDITOR EXPOSED FIELDS

    [SerializeField] List<Collider> _damageableColliders = new();

    #endregion

    #region PROPERTIES

    public List<Collider> DamageableColliders { get => _damageableColliders; }

    #endregion

    private void Start()
    {
        Physics.IgnoreCollision(GetComponent<Collider>(), characterControllerCollider, true);
    }

    private void OnTriggerEnter(Collider other)
    {
        DamageableColliders.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (DamageableColliders.Contains(other))
        {
            DamageableColliders.Remove(other);
        }
    }

}
