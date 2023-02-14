using UnityEngine;
using Mirror;

[RequireComponent(typeof(PlayerNetworkedState))]
public class ServerGhost : NetworkBehaviour
{

    PlayerNetworkedState playerNetworkedState;
    [SerializeField] GameObject ghostPrefab;
    GameObject serverGhost;
    Animator ghostAnimator;

    static int forwardHash = Animator.StringToHash("Forward");
    static int rightHash = Animator.StringToHash("Right");

    public override void OnStartLocalPlayer()
    {

        playerNetworkedState = this.GetComponent<PlayerNetworkedState>();

        if (this.isActiveAndEnabled)
        {
            serverGhost = Instantiate(ghostPrefab);
            ghostAnimator = serverGhost.GetComponent<Animator>();
        }
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            serverGhost.transform.position = playerNetworkedState.latestServerState.Position;
            serverGhost.transform.rotation = playerNetworkedState.latestServerState.Rotation;
            ghostAnimator.SetFloat(forwardHash, transform.InverseTransformDirection(playerNetworkedState.latestServerState.CurrentVelocity).z);
            ghostAnimator.SetFloat(rightHash, transform.InverseTransformDirection(playerNetworkedState.latestServerState.CurrentVelocity).x);
        }
    }

}
