using UnityEngine;

namespace Utils
{
    public static class MirkwoodDebug
    {

        static string prefix = $"[Mirkwood]: ";

        public static void LogDebug(string logOutput)
        {
            Debug.Log($"[DEBUG][{System.DateTime.Now}]" + prefix + logOutput);
        }

        public static void LogWarning(string logOutput)
        {
            Debug.LogWarning($"[WARNING][{System.DateTime.Now}]" + prefix + logOutput);
        }

        public static void LogError(string logOutput)
        {
            Debug.LogError($"[ERROR][{System.DateTime.Now}]" + prefix + logOutput);
        }
    }
}