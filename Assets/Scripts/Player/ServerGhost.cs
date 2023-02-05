using UnityEngine;
using Mirror;

public class ServerGhost : NetworkBehaviour
{

    [SyncVar] Vector3 serverPosition;
    [SyncVar] Quaternion serverRotation;
    [SerializeField] GameObject ghostPrefab;
    GameObject serverGhost;


    public override void OnStartLocalPlayer()
    {
        serverGhost = Instantiate(ghostPrefab);
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
