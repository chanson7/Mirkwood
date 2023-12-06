using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AreaOfEffect : MonoBehaviour
{

    #region PROPERTIES

    public List<Transform> Collisions = new();

    #endregion

    private void Start()
    {
        Physics.IgnoreCollision(GetComponent<Collider>(), GetComponentInParent<CharacterController>(), true); //ignore this player's character collider
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Collider>())
            Collisions.Add(other.gameObject.GetComponent<Transform>());
    }

    private void OnTriggerExit(Collider other)
    {
        if (Collisions.Contains(other.gameObject.GetComponent<Transform>()))
            Collisions.Remove(other.gameObject.GetComponent<Transform>());
    }


}
