using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeCollision : MonoBehaviour
{

    List<Collider> playerCollisions = new List<Collider>();

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"..{other.name} is in {this.name}'s melee range");
        playerCollisions.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (playerCollisions.Contains(other))
        {
            Debug.Log($"..{other.name} is no longer in {this.name}'s melee range");
            playerCollisions.Remove(other);
        }
    }

}
