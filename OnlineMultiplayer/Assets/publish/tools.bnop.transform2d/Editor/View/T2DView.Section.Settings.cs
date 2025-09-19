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
        private void RenderSettings()
        {
            var foldingChanged = FoldoutSection(ref _data.Settings.PreferencesPanelIsOpen, "Preferences", () =>
            {
                EditorGUI.BeginChangeCheck();
                _data.Settings.ProjectPpu = EditorGUILayout.FloatField("Project PPU", _data.Settings.ProjectPpu);
                _data.Settings.ProjectPpu = Mathf.Max(0.001f, _data.Settings.ProjectPpu);
                _data.Settings.SnapToPixel = EditorGUILayout.Toggle("Snap to Pixel", _data.Settings.SnapToPixel);
                _data.Settings.ShowGuides = EditorGUILayout.Toggle("Show Guides", _data.Settings.ShowGuides);
                DisableIf(!_data.Settings.ShowGuides, () =>
                {
                    _data.Settings.SnapToGuidesIfShown = EditorGUILayout.Toggle("Snap to Guides", _data.Settings.SnapToGuidesIfShown);
                });
                if (EditorGUI.EndChangeCheck())
                    DispatchSettingsChange();
            });
            
            if (foldingChanged)
                DispatchSettingsChange();
        }
    }
}
