using HeathenEngineering.SteamworksIntegration;
using UnityEngine;

public class SteamInviteManager : MonoBehaviour
{
    public void HandleLaunchFromLobbyInvitation(LobbyData lobbyData)
    {
        Debug.Log($"{lobbyData.Name} invite received!");
    }
}
