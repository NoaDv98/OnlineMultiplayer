//----------------------------------------------
// Transform2D
// Copyright © 2024 BaconOppenheim™
//----------------------------------------------

using tools.bnop.Transform2d.Data;
using UnityEditor;
using UnityEngine;

namespace tools.bnop.Transform2d.View
{
    public partial class T2DView
    {
        private bool _useSpacing;
        private float _spacingToUse;

        private void RenderDistribute()
        {
            if (_data.ObjectCount < 2) return;
            var foldingChanged = FoldoutSection(ref _data.Settings.DistributePanelIsOpen, "Distribute", () =>
            {

                var spacing = _useSpacing
                    ? _data.Settings.UnitType == UnitType.Pixels ? T2DScreen.ToUnits(_spacingToUse) : _spacingToUse
                    : float.NaN;
                
                EditorGUILayout.BeginHorizontal(Styles.Field);
                {
                    _useSpacing = GUILayout.Toggle(_useSpacing, "Use Fixed Spacing", GUILayout.ExpandWidth(false));
                    GUI.enabled = _useSpacing;

                    Space();

                    _spacingToUse = CoordinateField("", _spacingToUse, _data.Settings.UnitType, GUILayout.Width(60));
                    GUI.enabled = true;
                }
                EditorGUILayout.EndHorizontal();

                Space();
                
                EditorGUILayout.BeginHorizontal();

                if (IconButton(IconButtonContent(Icons.DistributeLeft,
                        "Distribute Left Edges")))
                    DispatchClickDistribute(DistributeOptions.Left, spacing);

                GUILayout.FlexibleSpace();

                if (IconButton(IconButtonContent(Icons.DistributeHorizontalCenters,
                        "Distribute Horizontal Centers")))
                    DispatchClickDistribute(DistributeOptions.HorizontalCenters, spacing);

                GUILayout.FlexibleSpace();

                if (IconButton(IconButtonContent(Icons.DistributeRight,
                        "Distribute Right Edges")))
                    DispatchClickDistribute(DistributeOptions.Right, spacing);

                GUILayout.FlexibleSpace();

                if (IconButton(IconButtonContent(Icons.DistributeHorizontalSpace,
                        "Distribute Horizontal Space")))
                    DispatchClickDistribute(DistributeOptions.HorizontalSpacing, spacing);

                GUILayout.FlexibleSpace();

                if (IconButton(IconButtonContent(Icons.DistributeTop,
                        "Distribute Top Edges")))
                    DispatchClickDistribute(DistributeOptions.Top, spacing);

                GUILayout.FlexibleSpace();

                if (IconButton(IconButtonContent(Icons.DistributeVerticalCenters,
                        "Distribute Vertical Centers")))
                    DispatchClickDistribute(DistributeOptions.VerticalCenters, spacing);

                GUILayout.FlexibleSpace();

                if (IconButton(IconButtonContent(Icons.DistributeBottom,
                        "Distribute Bottom Edges")))
                    DispatchClickDistribute(DistributeOptions.Bottom, spacing);

                GUILayout.FlexibleSpace();

                if (IconButton(
                        IconButtonContent(Icons.DistributeVerticalSpace, "Distribute Vertical Space")))
                    DispatchClickDistribute(DistributeOptions.VerticalSpacing, spacing);

                EditorGUILayout.EndHorizontal();
            });

            if (foldingChanged)
                DispatchSettingsChange();
        }
    }
}
