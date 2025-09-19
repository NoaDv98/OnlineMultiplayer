//----------------------------------------------
// Transform2D
// Copyright © 2024 BaconOppenheim™
//----------------------------------------------

using UnityEditor;
using UnityEngine;

namespace tools.bnop.Transform2d.Service
{
    public enum EditorWindowDisableCause 
    {
        Unknown,
        ScriptRecompilation,
        EditorQuitting,
        SceneViewMaximized, 
        OtherReason 
    }

    public static class T2DEditorWindowMonitor
    {
        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            EditorApplication.quitting -= HandleEditorQuitting;
            EditorApplication.quitting += HandleEditorQuitting;
        }

        private static void HandleEditorQuitting()
        {
            IsEditorQuitting = true;
        }

        private static bool IsEditorQuitting { get; set; } = false;

        public static EditorWindowDisableCause GetDisableCause(EditorWindow currentWindow)
        {
            if (EditorApplication.isCompiling)
            {
                return EditorWindowDisableCause.ScriptRecompilation;
            }

            if (IsEditorQuitting)
            {
                return EditorWindowDisableCause.EditorQuitting;
            }

            var sceneViewInstances = SceneView.sceneViews;
            if (sceneViewInstances == null) return EditorWindowDisableCause.OtherReason;
            foreach (var viewObject in sceneViewInstances) 
            {
                var sv = viewObject as SceneView;
                if (sv == null) continue; 
                try
                {
                    if (sv.maximized)
                    {
                        return EditorWindowDisableCause.SceneViewMaximized;
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning(
                        $"EditorWindowDisableAnalyzer: Error checking maximized state of SceneView '{sv.titleContent?.text ?? "Unknown"}': {ex.Message}");
                }
            }

            return EditorWindowDisableCause.OtherReason;
        }
    }
}
