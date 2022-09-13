#if UNITY_EDITOR
using UnityEditor;
using System.Diagnostics;
using System.IO;

public class BuildScript
{
    static string[] scenes = new[] { "Assets/Scenes/_Start.unity", "Assets/Scenes/MainMenu.unity", "Assets/Scenes/Arena.unity" };

    [MenuItem("Client/Build Client (Windows)")]
    public static void BuildWindowsClient()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = scenes;
        buildPlayerOptions.locationPathName = "Builds/Windows/Client/Mirkwood.exe";
        buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
        buildPlayerOptions.options = BuildOptions.CompressWithLz4HC;

        UnityEngine.Debug.Log("..Building Client (Windows)...");
        BuildPipeline.BuildPlayer(buildPlayerOptions);
        UnityEngine.Debug.Log("..Built Client (Windows)");
    }

    [MenuItem("Server/Build Server (Windows)")]
    public static void BuildWindowsServer()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = scenes;
        buildPlayerOptions.locationPathName = "Builds/Windows/Server/MirkwoodServer.exe";
        buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
        buildPlayerOptions.subtarget = (int)StandaloneBuildSubtarget.Server;

        UnityEngine.Debug.Log("..Building Server (Windows)...");
        BuildPipeline.BuildPlayer(buildPlayerOptions);
        UnityEngine.Debug.Log("..Built Server (Windows)");
    }

    [MenuItem("Client/Run Local Test Client")]
    public static void RunLocalTestClient()
    {
        using (Process process = new Process())
        {
            process.StartInfo.FileName = Directory.GetCurrentDirectory() + "/Builds/Windows/Client/Mirkwood.exe";
            process.StartInfo.Arguments = "-localTestClient";

            process.Start();

            UnityEngine.Debug.Log($"..Running Local Test Client");
        }

    }

    [MenuItem("Client/Run Test Client")]
    public static void RunAwsTestClient()
    {
        using (Process process = new Process())
        {
            process.StartInfo.FileName = Directory.GetCurrentDirectory() + "/Builds/Windows/Client/Mirkwood.exe";
            process.StartInfo.Arguments = "-testClient";

            process.Start();

            UnityEngine.Debug.Log($"..Running Test Client");
        }

    }

    [MenuItem("Server/Run Local Test Server")]
    public static void RunLocalTestServer()
    {
        using (Process process = new Process())
        {
            process.StartInfo.FileName = Directory.GetCurrentDirectory() + "/Builds/Windows/Server/MirkwoodServer.exe";
            process.StartInfo.Arguments = "-localTestServer";

            process.Start();

            UnityEngine.Debug.Log($"..Running Local Test Server");
        }

    }

    [MenuItem("Server/Upload Server Build to AWS")]
    public static void UploadServerToAws()
    {
        string region = "us-east-2";

        using (Process process = new Process())
        {
            process.StartInfo.FileName = "CMD.exe";
            process.StartInfo.Arguments = $"/C \"aws gamelift upload-build --name MirkwoodServer --build-version 0.0.1 --build-root {Directory.GetCurrentDirectory()}/Builds/Windows/Server --operating-system WINDOWS_2012 --region {region}";

            process.Start();

            UnityEngine.Debug.Log($"..Uploaded Server Build to {region}");
        }

    }

    [MenuItem("Client/Upload Client Build to Steam")]
    public static void UploadClientToSteam()
    {

        using (Process process = new Process())
        {
            process.StartInfo.FileName = "CMD.exe";
            process.StartInfo.Arguments = $"/C \"C:/Users/hanso/Dev/Steamworks SDK/sdk/tools/ContentBuilder/builder/steamcmd.exe  +login chanson71 \"7CHRCHR19jm?\" +run_app_build \"C:/Users/hanso/Dev/Steamworks SDK/sdk/tools/ContentBuilder/scripts/app_836430.vdf\" +quit";

            process.Start();

            UnityEngine.Debug.Log($"..Uploaded Client Build to Steam");
        }

    }

}
#endif