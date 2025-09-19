//----------------------------------------------
// Transform2D
// Copyright © 2024 BaconOppenheim™
//----------------------------------------------

using System.Collections.Generic;
using System.Linq;
using tools.bnop.Transform2d.Data;
using UnityEditor;
using UnityEngine;

namespace tools.bnop.Transform2d.View
{
    public partial class T2DView
    {
        private void RenderToolbar()
        {
            EditorGUILayout.BeginHorizontal(Styles.ToolbarContainer);
            RenderUnitTypeToolbar();
            Space();
            RenderBoundsSourceToolbar();
            Space();
            RenderUnitSpaceToolbar();
            EditorGUILayout.EndHorizontal();
        }

        private void RenderUnitTypeToolbar()
        {
            var content = new[]
            {
                IconButtonContent(Icons.Pixels, "Units Type: Pixels"),
                IconButtonContent(Icons.Units, "Units Type: Units"),
            };

            var prevValue = _data.Settings.UnitType;
            _data.Settings.UnitType =
                (UnitType)Toolbar((int)_data.Settings.UnitType, content);
            if (prevValue != _data.Settings.UnitType)
                DispatchSettingsChange();
        }

        private struct BoundsSourceOption
        {
            public BoundsSource Type;
            public GUIContent GUIContent;
            public bool Enabled;
        }

        private void RenderBoundsSourceToolbar()
        {
            var selectorDisabled = _data.IsSingleSelection && (_data.HasCamera || _data.HasText);
            
            DisableIf(selectorDisabled, () =>
            {
                List<BoundsSourceOption> options = new()
                {
                    new BoundsSourceOption()
                    {
                        Type = BoundsSource.Collider,
                        GUIContent = IconButtonContent(Icons.Collider, "Bounds: Use Collider 2D"),
                        Enabled = _data.HasCollider || _data.IsNoSelection
                    },
                    new BoundsSourceOption()
                    {
                        Type = BoundsSource.Sprite,
                        GUIContent = IconButtonContent(Icons.Sprite, "Bounds: Use Sprite Renderer"),
                        Enabled = _data.HasRenderer || _data.IsNoSelection
                    },
                    new BoundsSourceOption()
                    {
                        Type = BoundsSource.None,
                        GUIContent = IconButtonContent(Icons.Transform, "No bounds: Unity's default behavior. Use Transform origin as anchor"),
                        Enabled =  true
                    },
                };

                var content = options.Select(option => option.GUIContent).ToArray();
                var boundsSourceEnabled = options.Select(option => option.Enabled).ToArray();
                var current = options.FindIndex(option => option.Type == _data.ActiveBoundsSource);

                var selected = Toolbar(current, content, boundsSourceEnabled);
                if (!selectorDisabled && selected > -1)
                {
                    if (options[selected].Type != _data.ActiveBoundsSource)
                    {
                        _data.Settings.BoundsSource = options[selected].Type;
                        DispatchSettingsChange();
                    }
                }
            });

        }

        private void RenderUnitSpaceToolbar()
        {
            var activeUnitSpace =
                _data.IsMultiSelection ? UnitSpace.Global : _data.Settings.UnitSpace;

            var content = new[]
            {
                IconButtonContent(Icons.Local, "Units Space: Local"),
                IconButtonContent(Icons.Global, "Units Space: Global"),
            };

            var unitSpaceEnabled = new[]
            {
                !_data.IsMultiSelection,
                true
            };

            _data.Settings.UnitSpace =
                (UnitSpace)Toolbar((int)activeUnitSpace, content, unitSpaceEnabled);
            if (_data.Settings.UnitSpace != activeUnitSpace)
                DispatchSettingsChange();
        }
    }
}
