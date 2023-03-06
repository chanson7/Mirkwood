using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using Mirror;

public class DamageOverTimeZone : NetworkBehaviour
{

    [SerializeField] int damagePerSecond = 1;
    Dictionary<GameObject, Coroutine> objectsTakingDamage = new Dictionary<GameObject, Coroutine>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent(typeof(PlayerHealth)))
        {
            if (isServer)
                objectsTakingDamage.Add(other.gameObject, StartCoroutine(DealDamageOverTime(other.gameObject)));

            Debug.Log($"..{other.name} entered damage over time zone");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (objectsTakingDamage.Any(item => item.Key == other.gameObject))
        {
            if (isServer)
            {
                StopCoroutine(objectsTakingDamage[other.gameObject]);
                objectsTakingDamage.Remove(other.gameObject);
            }

            Debug.Log($"..removed {other.gameObject} from list of objects taking damage");
        }
    }

    [Server]
    IEnumerator DealDamageOverTime(GameObject obj)
    {
        while (true)
        {
            obj.GetComponent<PlayerHealth>().TakeDamage(damagePerSecond);

            yield return new WaitForSeconds(1);
        }
    }

}
