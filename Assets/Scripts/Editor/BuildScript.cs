#if UNITY_EDITOR
using System;
using UnityEditor;
using System.Diagnostics;
using UnityEngine;

public class BuildScript
{
    static string[] scenes = new[] { "Assets/Scenes/_Start.unity", "Assets/Scenes/MainMenu.unity" };

    [MenuItem("Build/Build Client (Windows)")]
    public static void BuildWindowsClient()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = scenes;
        buildPlayerOptions.locationPathName = "Builds/Windows/Client/Mirkwood.exe";
        buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
        buildPlayerOptions.options = BuildOptions.CompressWithLz4HC;

        Console.WriteLine("..Building Client (Windows)...");
        BuildPipeline.BuildPlayer(buildPlayerOptions);
        Console.WriteLine("..Built Client (Windows).");
    }

    [MenuItem("Build/Build Server (Windows)")]
    public static void BuildWindowsServer()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = scenes;
        buildPlayerOptions.locationPathName = "Builds/Windows/Client/MirkwoodServer.exe";
        buildPlayerOptions.target = (UnityEditor.BuildTarget)StandaloneBuildSubtarget.Server;

        Console.WriteLine("..Building Server (Windows)...");
        BuildPipeline.BuildPlayer(buildPlayerOptions);
        Console.WriteLine("..Built Server (Windows).");
    }

}
#endif