//----------------------------------------------
// Transform2D
// Copyright © 2024 BaconOppenheim™
//----------------------------------------------

using UnityEditor;
using UnityEngine;

namespace tools.bnop.Transform2d.View
{
    public partial class T2DView
    {
        private void Render3DMode()
        {
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical();

            GUILayout.Label("3D Mode is Currently Unsupported");
            Space(6);
            if (GUILayout.Button("Switch to 2D Mode"))
            {
                SceneView.lastActiveSceneView.in2DMode = true;
            }

            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
        }
    }
}
