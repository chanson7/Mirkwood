using System.Collections.Generic;
using UnityEngine;

public class MeleeCollision : MonoBehaviour
{

    List<Collider> playerCollisions = new List<Collider>();
    [SerializeField] Collider characterControllerCollider;

    private void Start()
    {
        Physics.IgnoreCollision(this.GetComponent<Collider>(), characterControllerCollider, true);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"..{other.name} is in melee range!");
        playerCollisions.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (playerCollisions.Contains(other))
        {
            playerCollisions.Remove(other);
        }
    }

}
