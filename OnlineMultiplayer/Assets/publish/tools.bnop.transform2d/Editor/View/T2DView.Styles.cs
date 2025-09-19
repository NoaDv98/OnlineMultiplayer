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
        private struct StyleProperties
        {
            public const float LabelWidthNormalized = 0.35f;
            public const int LabelWidthMin = 112;
            public const int PivotWidgetSize = 42;
            public const int ToolbarHeight = 22;
        }

        private struct Styles
        {
            public static readonly GUIStyle BorderTop = new()
            {
                normal =
                {
                    background = (Texture2D)Image(Backgrounds.BorderTop)
                },
                border = new RectOffset(0, 0, 2, 0)
            };

            public static readonly GUIStyle IndentedSection = new()
            {
                padding = new RectOffset(18, 3, 8, 8)
            };
            
            public static readonly GUIStyle IndentedBorderedSection = new(BorderTop)
            {
                padding = IndentedSection.padding,
            };

            public static readonly GUIStyle ToolbarContainer = new()
            {
                padding = new RectOffset(3, 3, 2, 0),
                margin = UniformRectOffset(0)
            };

            public static readonly GUIStyle SelectedObjectsSection = new()
            {
                padding = UniformRectOffset(7, 6)
            };
            
            public static readonly GUIStyle TransformSection = new()
            {
                padding = UniformRectOffset(0, 4),
            };

            public static readonly GUIStyle Toolbar = new(GUI.skin.button)
            {
                margin = UniformRectOffset(0),
                fixedHeight = StyleProperties.ToolbarHeight,
            };

            public static readonly GUIStyle PivotWidget = new()
            {
                normal =
                {
                    background = (Texture2D)Image(Backgrounds.PivotWidget),
                },
                fixedWidth = StyleProperties.PivotWidgetSize,
                fixedHeight = StyleProperties.PivotWidgetSize,
                padding = UniformRectOffset(0),
                margin = UniformRectOffset(0),
                stretchWidth = false
            };

            public static readonly GUIStyle PivotWidgetButton = new()
            {
                margin = UniformRectOffset(0),
                fixedHeight = 14,
                fixedWidth = 14,
                normal =
                {
                    background = (Texture2D)Image(Backgrounds.PivotWidgetButton)
                },
                hover =
                {
                    background = (Texture2D)Image(Backgrounds.PivotWidgetButtonActive)
                }
            };

            public static readonly GUIStyle PivotWidgetButtonActive = new(PivotWidgetButton)
            {
                normal =
                {
                    background = (Texture2D)Image(Backgrounds.PivotWidgetButtonActive)
                }
            };

            public static readonly GUIStyle TransformFields = new()
            {
                padding = new RectOffset(0, 0, 1, 0)
            };
            
            public static readonly GUIStyle Field = new ()
            {
                fixedHeight = StyleProperties.ToolbarHeight
            };

            public static readonly GUIStyle Message = new(EditorStyles.helpBox)
            {
                padding = UniformRectOffset(8),
                margin = UniformRectOffset(0)
            };
        }
    }
}
