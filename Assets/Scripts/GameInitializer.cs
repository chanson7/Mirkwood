using UnityEngine.SceneManagement;
using UnityEngine;
using System;
using HeathenEngineering.SteamworksIntegration;

public class GameInitializer : MonoBehaviour
{

    [SerializeField] MirkwoodNetworkManager networkManager;
    [SerializeField] SteamworksBehaviour steamworksBehaviour;

    [SerializeField] bool disableServerAuth = false;
    [SerializeField] bool autoStartServer = false;
    [SerializeField] bool disableSteam = false;

    void Awake()
    {
        EvaluateCommandLineArgs();
        ApplySettings();
    }

    private void Start()
    {
        SceneManager.LoadScene("MainMenu");
    }

    void EvaluateCommandLineArgs()
    {
        foreach (string arg in Environment.GetCommandLineArgs())
        {
            switch (arg)
            {
                case "-autoStartServer":
                    autoStartServer = true;
                    break;

                case "-disableServerAuth":
                    disableServerAuth = true;
                    break;

                case "-disableSteam":
                    disableSteam = true;
                    break;

                case "-localTestClient":
                    disableServerAuth = true;
                    disableSteam = true;
                    break;

                case "-localTestServer":
                    disableServerAuth = true;
                    autoStartServer = true;
                    disableSteam = true;
                    break;

                default:
                    break;
            }
        }
    }

    void ApplySettings()
    {
        if (disableServerAuth)
        {
            Debug.Log("..Disabling Authentication");
            networkManager.authenticator = null;
        }
        if (autoStartServer)
        {
            Debug.Log("..Server will automatically start");
            networkManager.StartServer();
        }
        if (disableSteam)
        {
            Debug.Log("..Disabling Steam Integration");
            steamworksBehaviour.enabled = false;
        }
    }

}
