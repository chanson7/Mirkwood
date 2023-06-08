using Steamworks;
using System.Text;
using UnityEngine;
using Fusion.Photon.Realtime;

public class SteamAuthenticator : MonoBehaviour
{

    [SerializeField] FusionAuth _fusionAuth;

    void Awake()
    {
        AuthenticateWithSteam();
    }

    public void AuthenticateWithSteam()
    {
        AuthenticationValues authValues = new AuthenticationValues();
        HAuthTicket ticket;

        authValues.UserId = SteamUser.GetSteamID().ToString();
        authValues.AuthType = CustomAuthenticationType.Steam;
        authValues.AddAuthParameter("ticket", GetSteamAuthTicket(out ticket));

        _fusionAuth.UpdateAuthValues(authValues);
    }

    public string GetSteamAuthTicket(out HAuthTicket ticket)
    {
        byte[] ticketByteArray = new byte[1024];
        uint ticketSize;

        ticket = SteamUser.GetAuthSessionTicket(ticketByteArray, ticketByteArray.Length, out ticketSize);

        System.Array.Resize(ref ticketByteArray, (int)ticketSize);
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < ticketSize; i++)
        {
            sb.AppendFormat("{0:x2}", ticketByteArray[i]);
        }

        return sb.ToString();
    }
}
