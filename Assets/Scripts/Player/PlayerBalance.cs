using UnityEngine;
using Mirror;
public class PlayerBalance : NetworkBehaviour
{

    [SyncVar]
    [Range(-1f, 1f)]
    [SerializeField]
    float leftRightBalance = 0f;

    [SyncVar]
    [Range(-1, 1f)]
    [SerializeField]
    float forwardsBackwardsBalance = 0f;

}
