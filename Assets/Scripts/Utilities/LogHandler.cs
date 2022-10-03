using UnityEngine;
using System;
using System.IO;

//
public class LogHandler : MonoBehaviour
{
    private StreamWriter writer;
    void Awake()
    {
        writer = File.AppendText("ServerLog.txt");

        Application.logMessageReceived += HandleLog;
    }

    private void HandleLog(string condition, string stackTrace, LogType type)
    {
        var logEntry = string.Format("\n{0} {1} {2} {3}"
            , DateTime.Now, type, condition, stackTrace);
        writer.Write(logEntry);
    }

    void OnDestroy()
    {
        writer.Close();
    }
}