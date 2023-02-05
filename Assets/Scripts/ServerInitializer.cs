using UnityEngine.SceneManagement;
using UnityEngine;
using System;

public class ServerInitializer : MonoBehaviour
{

    [SerializeField] MirkwoodNetworkManager networkManager;
    [SerializeField] ServerLaunchConfiguration serverConfig;

    void Awake()
    {
        EvaluateCommandLineArgs();
    }

    void EvaluateCommandLineArgs()
    {
        foreach (string arg in Environment.GetCommandLineArgs())
        {
            switch (arg)
            {
                case "-autoStartServer":
                    serverConfig.autoStartServer = true;
                    break;

                case "-disableServerAuth":
                    serverConfig.disableServerAuth = true;
                    break;

                case "-localTestServer":
                    serverConfig.disableServerAuth = true;
                    serverConfig.autoStartServer = true;
                    break;

                default:
                    break;
            }
        }
    }

}
