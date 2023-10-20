using HeathenEngineering.SteamworksIntegration.UI;
using UnityEngine;
using TMPro;
public class LobbyUIController : MonoBehaviour
{

    [SerializeField] TMP_Text statusText;
    QuickMatchLobbyControl quickMatchLobbyControl;

    public void UpdateStatusText()
    {
        switch (quickMatchLobbyControl.WorkingStatus)
        {
            case QuickMatchLobbyControl.Status.Searching:
                statusText.SetText($"Searching...");
                break;
            case QuickMatchLobbyControl.Status.WaitingForStart:
                statusText.SetText($"{quickMatchLobbyControl.Lobby.Members.Length}/{quickMatchLobbyControl.Lobby.MaxMembers} Players Found");
                break;
            case QuickMatchLobbyControl.Status.Starting:
                statusText.SetText($"Match Starting!");
                break;
            default:
                break;
        }
    }

    public void StartQuickMatchLobby(int numPlayers)
    {
        quickMatchLobbyControl.createArguments.slots = numPlayers;
        quickMatchLobbyControl.RunQuckMatch();
    }

    private void Awake()
    {
        quickMatchLobbyControl = GetComponent<QuickMatchLobbyControl>();
    }

}
