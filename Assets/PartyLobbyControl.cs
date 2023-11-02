using HeathenEngineering.SteamworksIntegration;
using UnityEngine;

public class PartyLobbyControl : MonoBehaviour
{

    public void HandleLobbyStuffWhatever(LobbyData lobby, UserData user)
    {
        lobby.Join((result, error) =>
        {
            if (!error)
                Debug.Log($"We have joined the lobby {lobby.Name}");
        });
    }

}