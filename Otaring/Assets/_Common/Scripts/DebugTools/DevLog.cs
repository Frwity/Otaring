using UnityEngine;

namespace Com.RandomDudes.Debug
{
    public static class DevLog
    {
        public static void Message(string message)
        {
            if (UnityEngine.Debug.isDebugBuild)
                UnityEngine.Debug.Log(message);
        }

        public static void Message(string message, Object context)
        {
            if (UnityEngine.Debug.isDebugBuild)
                UnityEngine.Debug.Log(message, context);
        }

        public static void Warning(string message)
        {
            if (UnityEngine.Debug.isDebugBuild)
                UnityEngine.Debug.LogWarning(message);
        }

        public static void Warning(string message, Object context)
        {
            if (UnityEngine.Debug.isDebugBuild)
                UnityEngine.Debug.LogWarning(message, context);
        }

        public static void Error(string message)
        {
            if (UnityEngine.Debug.isDebugBuild)
                UnityEngine.Debug.LogError(message);
        }

        public static void Error(string message, Object context)
        {
            if (UnityEngine.Debug.isDebugBuild)
                UnityEngine.Debug.LogError(message, context);
        }
    }
}