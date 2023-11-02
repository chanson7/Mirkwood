using HeathenEngineering.SteamworksIntegration;
using HeathenEngineering.SteamworksIntegration.UI;
using System.Collections.Generic;
using UnityEngine;

public class PartyLobbyControl : MonoBehaviour
{
    [SerializeField] LobbyManager lobbyManager;

    private readonly List<IChatMessage> chatMessages = new List<IChatMessage>();

    public void JoinPartyLobby(LobbyData lobby, UserData user)
    {
        lobby.Join((result, error) =>
        {
            if (!error)
            {
                Debug.Log($"We have joined the lobby {lobby.Name}");
                lobbyManager.Lobby = lobby;
            }
        });
    }

}