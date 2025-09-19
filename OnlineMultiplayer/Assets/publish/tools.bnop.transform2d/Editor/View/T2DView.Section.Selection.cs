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
        private void RenderSelection()
        {
            var label = _data switch
            {
                { HasPrefab: true } => "Prefab Asset Selected",
                { HasNesting: true } => "Nested GameObjects Selected",
                { ObjectCount: 0 } => "No GameObjects Selected",
                { IsSingleSelection: true } => _data.ActiveGameObjectName,
                _ => $"{_data.ObjectCount} GameObjects"
            };

            var icon = _data switch
            {
                { IsSingleSelection: true, HasCamera: true } => Icons.Camera,
                { IsSingleSelection: true } => Icons.GameObject,
                { HasPrefab: true } => Icons.GameObject,
                { HasNesting: true } => Icons.Group,
                { ObjectCount: > 1} => Icons.Group,
                _ => Icons.NoSelection,
            };

            EditorGUILayout.BeginHorizontal(Styles.SelectedObjectsSection);
            {
                var prevIconSize = EditorGUIUtility.GetIconSize();
                EditorGUIUtility.SetIconSize(Vector2.one * 24);
                GUILayout.Label(Image(icon), GUILayout.ExpandWidth(false));
                EditorGUIUtility.SetIconSize(prevIconSize);
                GUILayout.Label(label, EditorStyles.boldLabel, GUILayout.Height(24));
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
