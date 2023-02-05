using UnityEngine;
using Mirror;

public class PlayerExtraction : NetworkBehaviour
{
    [SerializeField] int extractionTime;

    [Server]
    public void Extract()
    {
        if (extractionTime < 1)
        {
            connectionToClient.Disconnect();

            Debug.Log($"..{this.name} has extracted!");
        }
        else
        {
            extractionTime--;
            Debug.Log($"..{this.name} has {extractionTime} seconds until extraction");
        }
    }

}
