//----------------------------------------------
// Transform2D
// Copyright © 2024 BaconOppenheim™
//----------------------------------------------

using System.Linq;
using tools.bnop.Transform2d.Data;
using UnityEditor;
using UnityEngine;

namespace tools.bnop.Transform2d.View
{
    public partial class T2DView
    {
        private void RenderAlign()
        {
            if (_data.ObjectCount < 2) return;
            var foldingChanged = FoldoutSection(ref _data.Settings.AlignPanelIsOpen, "Align", () =>
            {
                EditorGUILayout.BeginHorizontal();
                if (IconButton(IconButtonContent(Icons.AlignLeft, "Align Left")))
                    DispatchClickAlign(AlignOptions.Left);

                GUILayout.FlexibleSpace();

                if (IconButton(
                        IconButtonContent(Icons.AlignHorizontalCenter, "Align Horizontal Center")))
                    DispatchClickAlign(AlignOptions.HorizontalCenter);

                GUILayout.FlexibleSpace();

                if (IconButton(IconButtonContent(Icons.AlignRight, "Align Right")))
                    DispatchClickAlign(AlignOptions.Right);

                GUILayout.FlexibleSpace();

                if (IconButton(IconButtonContent(Icons.AlignTop, "Align Top")))
                    DispatchClickAlign(AlignOptions.Top);

                GUILayout.FlexibleSpace();

                if (IconButton(IconButtonContent(Icons.AlignVerticalCenter, "Align Vertical Center")))
                    DispatchClickAlign(AlignOptions.VerticalCenter);

                GUILayout.FlexibleSpace();

                if (IconButton(IconButtonContent(Icons.AlignBottom, "Align AlignBottom")))
                    DispatchClickAlign(AlignOptions.Bottom);
                EditorGUILayout.EndHorizontal();
            });
            
            if (foldingChanged)
                DispatchSettingsChange();
        }
    }
}
