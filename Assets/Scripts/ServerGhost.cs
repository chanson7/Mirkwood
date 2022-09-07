using UnityEngine;
using Mirror;

public class ServerGhost : NetworkBehaviour
{

    [SyncVar] Vector3 serverPosition;

    [SyncVar] Quaternion serverRotation;
    [SerializeField] GameObject serverGhost;


    private void Start()
    {
        if (!base.isServer && !isLocalPlayer)
            Destroy(serverGhost);
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
