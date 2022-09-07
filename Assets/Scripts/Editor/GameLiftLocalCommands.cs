#if UNITY_EDITOR
using UnityEditor;
using System.Diagnostics;
using System;
using Newtonsoft.Json;
using System.IO;

public class GameLiftLocalCommands
{

    [MenuItem("GameLift/Upload Server Build (Windows)")]
    public static void UploadServerBuild()
    {
        // using (Process process = new Process())
        // {
        //     process.StartInfo.Arguments = "/C \"aws gamelift upload-build --name Mirkwood Server --build-version 0.0.1 --build-root \"Builds/Windows/Server/MirkwoodServer.exe\" --operating-system WINDOWS_2012 --region us-east-2";
        //     process.StartInfo.FileName = "CMD.exe";
        //     process.StartInfo.CreateNoWindow = true;

        //     process.Start();

        //     UnityEngine.Debug.Log($"..Windows Server uploaded to AWS");
        // }
    }

    // [MenuItem("GameLift/Create Player Session")]
    // public static void CreatePlayerSession()
    // {

    //     GameSession gameSession = JsonConvert.DeserializeObject<GameLiftGameSession>(File.ReadAllText($@"{Environment.CurrentDirectory}\GameSession.txt")).gameSession;

    //     string text = File.ReadAllText($@"{Environment.CurrentDirectory}\GameSession.txt");

    //     using (Process process = new Process())
    //     {
    //         process.StartInfo.Arguments = $"/C \"AWS gamelift create-player-session --player-id 12345 --endpoint-url http://localhost:8080 --game-session-id {gameSession.gameSessionId} > PlayerSession.txt\"";
    //         process.StartInfo.FileName = "CMD.exe";
    //         process.StartInfo.CreateNoWindow = true;

    //         process.Start();

    //         UnityEngine.Debug.Log($"..PlayerSession.txt File Updated");
    //     }
    // }
}

#endif