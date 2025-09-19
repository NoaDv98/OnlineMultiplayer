//----------------------------------------------
// Transform2D
// Copyright © 2024 BaconOppenheim™
//----------------------------------------------

using UnityEditor;

namespace tools.bnop.Transform2d.Service
{
    public class T2DEditorWindow : EditorWindow
    {
        public const float DefaultScreenPpu = 100;

        public static void ShowWindow()
        {
            GetWindow(typeof(T2DEditorWindow), false, "Transform2D");
        }

        private void OnEnable()
        {
            if (!Transform2D.Initialized)
            {
                EditorApplication.delayCall += Transform2D.Init;
            }

            Transform2D.EditorWindow = this;
        }

        private void OnDisable()
        {
            var cause = T2DEditorWindowMonitor.GetDisableCause(this);
            if (cause == EditorWindowDisableCause.SceneViewMaximized)
                Transform2D.EditorWindow = null;
            else
                Transform2D.Destroy();
        }

        private void OnGUI()
        {
            Transform2D.OnGUI();
        }
    }
}
