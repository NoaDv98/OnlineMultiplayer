//----------------------------------------------
// Transform2D
// Copyright © 2024 BaconOppenheim™
//----------------------------------------------

using System.Globalization;
using tools.bnop.Transform2d.Data;
using UnityEditor;
using UnityEngine;

namespace tools.bnop.Transform2d.View
{
    public partial class T2DView
    {
        private void RenderSpriteInfo()
        {
            if (_data is not { IsSingleSelection: true, HasRenderer: true }) return;
            
            var foldingChanged = FoldoutSection(ref _data.Settings.SpriteInfoPanelIsOpen, "Sprite Asset Info", () =>
            {
                var isPixels = _data.Settings.UnitType == UnitType.Pixels;
                var precision = isPixels
                    ? T2DSettingsData.FloatPrecisionPixels
                    : T2DSettingsData.FloatPrecisionUnits;
                var roundedValue = new Vector2(CleanFloat(_data.SpritePivot.x, precision), CleanFloat(_data.SpritePivot.y, precision));
                
                var ppuMatch = Mathf.Approximately(_data.Settings.ProjectPpu, _data.SpritePPUSetting);
                var pivotSuffix = isPixels && !ppuMatch ? $" ({_data.Settings.ProjectPpu} PPU)" : "";
                var pivotString = !isPixels
                    ? $"X: {roundedValue.x}un  Y: {roundedValue.y}un"
                    : $"X: {roundedValue.x}px  Y: {roundedValue.y}px{pivotSuffix}";
                EditorGUILayout.LabelField("Sprite Pivot", pivotString);

                
                var ppuString = _data.SpriteIsDownscaled 
                    ? $"{_data.SpritePPUSetting} (downscaled to {_data.SpritePPU})"
                    : $"{_data.SpritePPU}";

                EditorGUILayout.LabelField("Sprite PPU", ppuString);
                
                Space(2);

                if (GUILayout.Button("Open Sprite Editor"))
                    DispatchClickOpenSpriteEditor();

                if (GUILayout.Button("Select Sprite Asset"))
                    DispatchClickFindSpriteAsset();
            });
                
            if(foldingChanged)
                DispatchSettingsChange();
        }
    }
}
