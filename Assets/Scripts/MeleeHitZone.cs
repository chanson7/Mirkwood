using System.Collections.Generic;
using UnityEngine;

public class MeleeHitZone : MonoBehaviour
{

    #region EDITOR EXPOSED FIELDS

    [SerializeField] Collider characterControllerCollider;
    [SerializeField] List<Collider> _damageableObjectsInRange = new();

    #endregion

    #region PROPERTIES

    public List<Collider> DamageableObjectsInRange { get => _damageableObjectsInRange; }

    #endregion

    private void Start()
    {
        Physics.IgnoreCollision(GetComponent<Collider>(), characterControllerCollider, true); //ignore this player's character collider
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<Collider>())
            DamageableObjectsInRange.Add(other.gameObject.GetComponent<Collider>());
    }

    private void OnTriggerExit(Collider other)
    {
        if (DamageableObjectsInRange.Contains(other.gameObject.GetComponent<Collider>()))
        {
            DamageableObjectsInRange.Remove(other.gameObject.GetComponent<Collider>());
        }
    }

}
