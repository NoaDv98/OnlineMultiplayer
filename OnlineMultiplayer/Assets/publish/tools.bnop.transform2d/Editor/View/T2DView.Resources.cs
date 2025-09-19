//----------------------------------------------
// Transform2D
// Copyright © 2024 BaconOppenheim™
//----------------------------------------------

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace tools.bnop.Transform2d.View
{
    public partial class T2DView
    {
        private struct Backgrounds
        {
            public const string BorderTop = "Textures/Border top";
            public const string PivotWidget = "Textures/Pivot widget/Pivot Widget BG";
            public const string PivotWidgetDisabled = "Textures/Pivot widget/Pivot Widget Disabled";
            public const string PivotWidgetButton = "Textures/Pivot widget/Pivot Button Default";
            public const string PivotWidgetButtonActive = "Textures/Pivot widget/Pivot Button Active";
            public const string IconButtonHovered = "Textures/Icon button/Icon button hovered";
            public const string IconButtonActive = "Textures/Icon button/Icon button active";
        }

        private struct Icons
        {
            public const string GameObject = "Icons/GameObject";
            public const string Group = "Icons/Group";
            public const string Camera = "Icons/Camera";
            public const string NoSelection = "Icons/NoSelection";
            
            public const string Pixels = "Icons/Pixels";
            public const string Units = "Icons/Units";
            public const string Percent = "Icons/Percent";
            public const string Degrees = "Icons/Degrees";
            
            public const string Local = "Icons/Local";
            public const string Global = "Icons/Global";
            
            public const string Transform = "Icons/Transform";
            public const string Sprite = "Icons/Sprite";
            public const string Collider = "Icons/Collider";
            
            public const string AlignBottom = "Icons/Align bottom";
            public const string AlignHorizontalCenter = "Icons/Align horizontal center";
            public const string AlignLeft = "Icons/Align left";
            public const string AlignRight = "Icons/Align right";
            public const string AlignTop = "Icons/Align top";
            public const string AlignVerticalCenter = "Icons/Align vertical center";
            
            public const string DistributeBottom = "Icons/Distribute bottom";
            public const string DistributeHorizontalCenters = "Icons/Distribute horizontal centers";
            public const string DistributeHorizontalSpace = "Icons/Distribute horizontal space";
            public const string DistributeLeft = "Icons/Distribute left";
            public const string DistributeRight = "Icons/Distribute right";
            public const string DistributeTop = "Icons/Distribute top";
            public const string DistributeVerticalCenters = "Icons/Distribute vertical centers";
            public const string DistributeVerticalSpace = "Icons/Distribute vertical space";
            

            public const string More = "d_more";
            public const string Info = "d_console.infoicon.sml";
            public const string Warning = "d_console.warnicon.sml";
            public const string Error = "d_console.erroricon.sml";
        }

        private static Dictionary<string, Texture> _textures;

        private static Texture Image(string name)
        {
            if (_textures == null)
                LoadResources();

            return _textures![name];
        }
        
        public static void LoadResources()
        {
            _textures = new Dictionary<string, Texture>();

            _textures[Backgrounds.BorderTop] =
                (Texture2D)Resources.Load($"Transform2D/{Backgrounds.BorderTop}");
            _textures[Backgrounds.PivotWidget] =
                (Texture2D)Resources.Load($"Transform2D/{Backgrounds.PivotWidget}");
            _textures[Backgrounds.PivotWidgetDisabled] =
                (Texture2D)Resources.Load($"Transform2D/{Backgrounds.PivotWidgetDisabled}");
            _textures[Backgrounds.PivotWidgetButton] =
                (Texture2D)Resources.Load($"Transform2D/{Backgrounds.PivotWidgetButton}");
            _textures[Backgrounds.PivotWidgetButtonActive] =
                (Texture2D)Resources.Load($"Transform2D/{Backgrounds.PivotWidgetButtonActive}");
            _textures[Backgrounds.IconButtonHovered] =
                (Texture2D)Resources.Load($"Transform2D/{Backgrounds.IconButtonHovered}");
            _textures[Backgrounds.IconButtonActive] =
                (Texture2D)Resources.Load($"Transform2D/{Backgrounds.IconButtonActive}");

            _textures[Icons.GameObject] =
                (Texture2D)Resources.Load($"Transform2D/{Icons.GameObject}");
            _textures[Icons.Group] =
                (Texture2D)Resources.Load($"Transform2D/{Icons.Group}");
            _textures[Icons.Camera] =
                (Texture2D)Resources.Load($"Transform2D/{Icons.Camera}");
            _textures[Icons.NoSelection] =
                (Texture2D)Resources.Load($"Transform2D/{Icons.NoSelection}");

            _textures[Icons.Pixels] =
                (Texture2D)Resources.Load($"Transform2D/{Icons.Pixels}");
            _textures[Icons.Units] =
                (Texture2D)Resources.Load($"Transform2D/{Icons.Units}");
            _textures[Icons.Percent] =
                (Texture2D)Resources.Load($"Transform2D/{Icons.Percent}");
            _textures[Icons.Degrees] =
                (Texture2D)Resources.Load($"Transform2D/{Icons.Degrees}");
            _textures[Icons.Local] =
                (Texture2D)Resources.Load($"Transform2D/{Icons.Local}");
            _textures[Icons.Global] =
                (Texture2D)Resources.Load($"Transform2D/{Icons.Global}");
            _textures[Icons.Transform] =
                (Texture2D)Resources.Load($"Transform2D/{Icons.Transform}");
            _textures[Icons.Sprite] =
                (Texture2D)Resources.Load($"Transform2D/{Icons.Sprite}");
            _textures[Icons.Collider] =
                (Texture2D)Resources.Load($"Transform2D/{Icons.Collider}");

            _textures[Icons.AlignBottom] =
                (Texture2D)Resources.Load($"Transform2D/{Icons.AlignBottom}");
            _textures[Icons.AlignHorizontalCenter] =
                (Texture2D)Resources.Load($"Transform2D/{Icons.AlignHorizontalCenter}");
            _textures[Icons.AlignLeft] =
                (Texture2D)Resources.Load($"Transform2D/{Icons.AlignLeft}");
            _textures[Icons.AlignRight] =
                (Texture2D)Resources.Load($"Transform2D/{Icons.AlignRight}");
            _textures[Icons.AlignTop] =
                (Texture2D)Resources.Load($"Transform2D/{Icons.AlignTop}");
            _textures[Icons.AlignVerticalCenter] =
                (Texture2D)Resources.Load($"Transform2D/{Icons.AlignVerticalCenter}");

            _textures[Icons.DistributeBottom] =
                (Texture2D)Resources.Load($"Transform2D/{Icons.DistributeBottom}");
            _textures[Icons.DistributeHorizontalCenters] =
                (Texture2D)Resources.Load($"Transform2D/{Icons.DistributeHorizontalCenters}");
            _textures[Icons.DistributeHorizontalSpace] =
                (Texture2D)Resources.Load($"Transform2D/{Icons.DistributeHorizontalSpace}");
            _textures[Icons.DistributeLeft] =
                (Texture2D)Resources.Load($"Transform2D/{Icons.DistributeLeft}");
            _textures[Icons.DistributeRight] =
                (Texture2D)Resources.Load($"Transform2D/{Icons.DistributeRight}");
            _textures[Icons.DistributeTop] =
                (Texture2D)Resources.Load($"Transform2D/{Icons.DistributeTop}");
            _textures[Icons.DistributeVerticalCenters] =
                (Texture2D)Resources.Load($"Transform2D/{Icons.DistributeVerticalCenters}");
            _textures[Icons.DistributeVerticalSpace] =
                (Texture2D)Resources.Load($"Transform2D/{Icons.DistributeVerticalSpace}");

            _textures[Icons.More] = EditorGUIUtility.IconContent(Icons.More).image;
            _textures[Icons.Info] = EditorGUIUtility.IconContent(Icons.Info).image;
            _textures[Icons.Warning] = EditorGUIUtility.IconContent(Icons.Warning).image;
            _textures[Icons.Error] = EditorGUIUtility.IconContent(Icons.Error).image;
        }
    }
}
