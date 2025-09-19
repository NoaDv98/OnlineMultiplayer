//----------------------------------------------
// Transform2D
// Copyright © 2024 BaconOppenheim™
//----------------------------------------------

using System;
using UnityEngine;

namespace tools.bnop.Transform2d.Service
{
    public static class T2DConsole
    {
        public static bool ShowInfo { get; set; } = false;
        public static bool ShowWarnings { get; set; } = true;
        public static bool ShowErrors { get; set; } = true;
        
        
        private const string Namespace = "Transform2D";

        public static void Log(string message)
        {
            if (ShowInfo)
                Debug.Log($"[{Namespace}] {message}");
        }

        public static void LogWarning(string message)
        {
            if (ShowWarnings)
                Debug.LogWarning($"[{Namespace}] {message}");
        }
        
        public static void LogError(string message)
        {
            if (ShowErrors)
                Debug.LogError($"[{Namespace}] {message}");
        }

        public static void HandleException(Exception exception)
        {
            var message = $"A general error has occured: {exception.Message}";
            LogError(message);
        }
    }
}
