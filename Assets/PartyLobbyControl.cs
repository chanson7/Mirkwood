using HeathenEngineering.SteamworksIntegration;
using UnityEngine;

public class PartyLobbyControl : MonoBehaviour
{

    public void HandleLobbyStuffWhatever(LobbyData lobby, UserData user)
    {
        Debug.Log($"{lobby} invite received!");
    }

}