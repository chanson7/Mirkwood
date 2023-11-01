using UnityEngine;
using HeathenEngineering.SteamworksIntegration.UI;
using HeathenEngineering.SteamworksIntegration;

public class SteamInvite : MonoBehaviour
{

    FriendProfile friend;
    LobbyData lobby;

    private void OnEnable()
    {
        friend = GetComponent<FriendProfile>();
        lobby = GetComponentInParent<LobbyManager>().Lobby;
    }

    public void InviteFriendToGroupLobby()
    {
        if (lobby != null)
        {
            Debug.Log($"inviting {friend.UserData.Nickname} to {lobby.Name}");

            lobby.InviteUserToLobby(friend.UserData);
        }
    }
}
