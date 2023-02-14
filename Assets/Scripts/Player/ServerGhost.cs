using UnityEngine;
using Mirror;

public class ServerGhost : NetworkBehaviour
{

    [SerializeField] PlayerPredictedTransform predictedTransform;
    [SerializeField] GameObject ghostPrefab;
    GameObject serverGhost;
    Animator ghostAnimator;

    static int forwardHash = Animator.StringToHash("Forward");
    static int rightHash = Animator.StringToHash("Right");

    public override void OnStartLocalPlayer()
    {
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
            serverGhost.transform.position = predictedTransform.latestServerState.Position;
            serverGhost.transform.rotation = predictedTransform.latestServerState.Rotation;
            ghostAnimator.SetFloat(rightHash, transform.InverseTransformDirection(predictedTransform.latestServerState.CurrentVelocity).x);
            ghostAnimator.SetFloat(forwardHash, transform.InverseTransformDirection(predictedTransform.latestServerState.CurrentVelocity).z);
        }
    }

}
