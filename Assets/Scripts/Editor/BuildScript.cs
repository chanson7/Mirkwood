#if UNITY_EDITOR
using UnityEditor;
using System.Diagnostics;

public class BuildScript
{
    static string[] scenes = new[] { "Assets/Scenes/_Start.unity", "Assets/Scenes/MainMenu.unity", "Assets/Scenes/Arena.unity" };

    [MenuItem("Build/Build Client (Windows)")]
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

    [MenuItem("Build/Build Server (Windows)")]
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

    [MenuItem("Build/Run Local Test Server")]
    public static void RunLocalTestServer()
    {
        using (Process process = new Process())
        {
            process.StartInfo.FileName = "Builds/Windows/Server/MirkwoodServer.exe";
            process.StartInfo.Arguments = "-localTest";

            process.Start();

            UnityEngine.Debug.Log($"..Running Local Test Server");
        }

    }

    [MenuItem("Build/Run Local Test Client")]
    public static void RunLocalTestClient()
    {
        using (Process process = new Process())
        {
            process.StartInfo.FileName = "Builds/Windows/Client/Mirkwood.exe";
            process.StartInfo.Arguments = "-disableSteam";

            process.Start();

            UnityEngine.Debug.Log($"..Running Local Test Client");
        }

    }

}
#endif