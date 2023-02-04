using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using Mirror;
public class ExtractionZone : NetworkBehaviour
{
    Dictionary<GameObject, Coroutine> extractingPlayers = new Dictionary<GameObject, Coroutine>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent(typeof(PlayerExtraction)))
        {
            if (isServer)
                extractingPlayers.Add(other.gameObject, StartCoroutine(ExtractPlayer(other.gameObject)));

            Debug.Log($"..{other.name} is extracting");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (extractingPlayers.Any(item => item.Key == other.gameObject))
        {
            if (isServer)
            {
                StopCoroutine(extractingPlayers[other.gameObject]);
                extractingPlayers.Remove(other.gameObject);
            }

            Debug.Log($"..removed {other.gameObject} is no longer extracting");
        }
    }

    [Server]
    IEnumerator ExtractPlayer(GameObject obj)
    {
        while (true)
        {
            obj.GetComponent<PlayerExtraction>().Extract();
            yield return new WaitForSeconds(1);
        }
    }
}
