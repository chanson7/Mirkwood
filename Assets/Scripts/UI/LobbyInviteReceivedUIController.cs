using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HeathenEngineering.SteamworksIntegration;

public class LobbyInviteReceivedUIController : MonoBehaviour
{

    LobbyData lobbyInvitedTo;

    public void HandleNewLobbyInvitation(LobbyData lobbyInvitedTo)
    {
        this.lobbyInvitedTo = lobbyInvitedTo;
    }

    public void AcceptLobbyInvite()
    {
        //lobbyInvitedTo
    }

    public void DeclineLobbyInvite() 
    { 
    
    }
}
