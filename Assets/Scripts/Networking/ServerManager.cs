using System.Collections.Generic;
using System;
using UnityEngine;
using Fusion;
using Fusion.Sockets;

public class ServerManager : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] protected int _tickRate;
    [SerializeField] NetworkRunner _runner;
    [SerializeField] int _playerCount;
    [SerializeField] FusionAuth _fusionAuth;

    void Awake()
    {
        ConfigureHeadlessFrameRate(_tickRate);
    }

    public async void OnFusionAuthUpdated()
    {

        StartGameResult result = await _runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Server,
            Scene = (int)Utils.SceneDefs.TESTING,
            SceneManager = _runner.gameObject.AddComponent<NetworkSceneManagerDefault>(),
            PlayerCount = _playerCount,
            AuthValues = _fusionAuth.authValues
        });

        if (result.Ok)
        {
            Log.Debug($"Runner Start DONE");
        }
        else
        {
            Utils.MirkwoodDebug.LogError($"Error while starting Server: {result.ShutdownReason}");
            Application.Quit(1);
        }
    }

    void ConfigureHeadlessFrameRate(int frameRate)
    {
        Application.targetFrameRate = frameRate;
        Utils.MirkwoodDebug.LogDebug($"Headless frame rate set to {frameRate}");
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Utils.MirkwoodDebug.LogDebug($"{player.PlayerId} joined the game");
    }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Utils.MirkwoodDebug.LogDebug($"{player.PlayerId} left the game");
    }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Utils.MirkwoodDebug.LogDebug($"Shutting down: {shutdownReason}");
    }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        Utils.MirkwoodDebug.LogDebug($"Received connect request from {request.RemoteAddress}");
    }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        Utils.MirkwoodDebug.LogError($"Connection from {remoteAddress} failed");
    }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
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
