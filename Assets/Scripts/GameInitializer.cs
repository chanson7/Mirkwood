using UnityEngine.SceneManagement;
using UnityEngine;
using System;

public class GameInitializer : MonoBehaviour
{

    [SerializeField] MirkwoodNetworkManager networkManager;
    [SerializeField] GameObject logGui;
    [SerializeField] bool disableServerAuth = false;
    [SerializeField] bool autoStartServer = false;
    [SerializeField] bool enableLogGui = false;

    void Awake()
    {
        EvaluateCommandLineArgs();
        ApplySettings();
    }

    void Start()
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

                case "-testClient":
                    enableLogGui = true;
                    break;

                case "-disableServerAuth":
                    disableServerAuth = true;
                    break;

                case "-enableLogGui":
                    enableLogGui = true;
                    break;

                case "-localTestClient":
                    disableServerAuth = true;
                    enableLogGui = true;
                    break;

                case "-localTestServer":
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
        if (autoStartServer)
        {
            Debug.Log("..Server will automatically start");
            networkManager.StartServer();
        }
        if (disableServerAuth)
        {
            Debug.Log("..Disabling Authentication");
            networkManager.authenticator = null;
        }
        if (enableLogGui)
        {
            Debug.Log("..Enabling log GUI");
            logGui.SetActive(true);
        }
    }

}
