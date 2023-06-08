using Fusion.Photon.Realtime;
using UnityEngine;

[CreateAssetMenu(fileName = "FusionAuth", menuName = "ScriptableObjects/FusionAuth", order = 2)]
public class FusionAuth : ScriptableObject
{
    public AuthenticationValues authValues;
    [SerializeField] ScriptableEvent _authValuesUpdatedEvent;

    public void UpdateAuthValues(AuthenticationValues authValues)
    {
        this.authValues = authValues;

        _authValuesUpdatedEvent.Raise();
    }
}
