using UnityEngine;
using Mirror;

public class ServerGhost : NetworkBehaviour
{

    [SyncVar] Vector3 serverPosition;

    [SyncVar] Quaternion serverRotation;
    [SerializeField] GameObject serverGhost;

    public override void OnStartLocalPlayer()
    {
        serverGhost.SetActive(true);
    }

    void Update()
    {
        if (isServer)
        {
            serverPosition = transform.position;
            serverRotation = transform.rotation;
        }
        if (isLocalPlayer)
        {
            serverGhost.transform.position = serverPosition;
            serverGhost.transform.rotation = serverRotation;
        }
    }

}
