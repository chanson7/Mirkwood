using UnityEngine;
using HeathenEngineering.SteamworksIntegration.UI;

public class MainMenuUIController : MonoBehaviour
{

    [SerializeField] QuickMatchLobbyControl lobbyControl;

    public void Quit()
    {
        Application.Quit();
    }

    public void StartQuickMatchLobby(int numPlayers)
    {
        lobbyControl.createArguments.slots = numPlayers;
        lobbyControl.RunQuckMatch();
    }

}
