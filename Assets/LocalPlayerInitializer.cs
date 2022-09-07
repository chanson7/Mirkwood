using UnityEngine;
using Mirror;

public class LocalPlayerInitializer : NetworkBehaviour
{

    [SerializeField] ScriptableEvent localPlayerStartedEvent;

    public override void OnStartLocalPlayer()
    {
        localPlayerStartedEvent.Raise();
    }
}
