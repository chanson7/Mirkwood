using HeathenEngineering.SteamworksIntegration;
using UnityEngine;

public class SteamInviteManager : MonoBehaviour
{
    public void HandleSteamInviteReceived(LobbyData lobbyData)
    {
        Debug.Log($"{lobbyData.Name} invite received!");
    }
}
