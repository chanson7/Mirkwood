using UnityEngine.SceneManagement;
using UnityEngine;
using System;

public class GameInitializer : MonoBehaviour
{

    [SerializeField] MirkwoodNetworkManager networkManager;

    [SerializeField] bool disableServerAuth = false;
    [SerializeField] bool autoStartServer = false;

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
                case "-disableServerAuth":
                    disableServerAuth = true;
                    break;

                case "-autoStartServer":
                    autoStartServer = true;
                    break;

                case "-localTest":
                    disableServerAuth = true;
                    autoStartServer = true;
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
    }

}
