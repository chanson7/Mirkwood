using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;
using Fusion;
using Fusion.Sockets;

public class ClientManager : MonoBehaviour, INetworkRunnerCallbacks
{

    [SerializeField] NetworkRunner _runner;
    [SerializeField] FusionAuth _fusionAuth;

    void Start()
    {
        _runner.ProvideInput = true;
        SceneManager.LoadScene("MainMenu");
    }

    public async void OnMatchRequested()
    {
        StartGameResult result = await _runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Client,
            Scene = (int)Utils.SceneDefs.TESTING,
            SceneManager = _runner.gameObject.AddComponent<NetworkSceneManagerDefault>(),
            AuthValues = _fusionAuth.authValues,
            DisableClientSessionCreation = true,
        });

        if (result.Ok)
        {
            Utils.MirkwoodDebug.LogDebug("Player Session Connected");
        }
        else
        {
            Utils.MirkwoodDebug.LogError($"Error while connecting to Player Session: {result.ShutdownReason}");
        }
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Utils.MirkwoodDebug.LogDebug($"PlayerID: {player.PlayerId} joined the game");
    }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Utils.MirkwoodDebug.LogDebug($"PlayerID: {player.PlayerId} left the game");
    }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Utils.MirkwoodDebug.LogDebug($"Shutting down: {shutdownReason}");
    }
    public void OnConnectedToServer(NetworkRunner runner)
    {
        Utils.MirkwoodDebug.LogDebug($"Connected to Server");
    }
    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        Utils.MirkwoodDebug.LogDebug($"Disconnected from Server");
    }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        Utils.MirkwoodDebug.LogError($"Connecting to {remoteAddress} failed");
    }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        Utils.MirkwoodDebug.LogDebug($"Auth Response Received: ");
    }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner)
    {
        Utils.MirkwoodDebug.LogDebug("Scene Load Done");
    }
    public void OnSceneLoadStart(NetworkRunner runner)
    {
        Utils.MirkwoodDebug.LogDebug("Scene Load Started");
    }
}
