using UnityEngine;
using System.Collections;
using Mirror;

public class PlayerHealth : NetworkBehaviour
{

    [SyncVar] int damageCounter;
    [SerializeField] int recoveryPerSecond = 1;

    public override void OnStartServer()
    {
        StartCoroutine(RecoverHealth());

        base.OnStartServer();
    }

    [Server]
    public void TakeDamage(int power)
    {
        damageCounter += power;
        Debug.Log($"..{gameObject.name} has taken {power} damage and has taken {damageCounter} total damage");
    }

    [Server]
    IEnumerator RecoverHealth()
    {
        while (true)
        {
            if (damageCounter > 1)
            {
                damageCounter -= recoveryPerSecond;
            }

            yield return new WaitForSeconds(1);
        }
    }

}
