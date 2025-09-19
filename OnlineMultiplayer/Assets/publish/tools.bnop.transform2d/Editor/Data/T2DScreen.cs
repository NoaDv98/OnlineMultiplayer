//----------------------------------------------
// Transform2D
// Copyright © 2024 BaconOppenheim™
//----------------------------------------------

using System;
using tools.bnop.Transform2d.Service;
using UnityEngine;

    
namespace tools.bnop.Transform2d.Data
{
    public abstract class T2DScreen
    {
        public static float PPU { get; set; } = T2DEditorWindow.DefaultScreenPpu;
        
        private static Vector2 Vector2Round(Vector2 value)
        {
            return new Vector2(Mathf.Round(value.x), Mathf.Round(value.y));
        }
        
        public static Vector2 ToPixels(Vector2 units, bool snapToPixel = false, float overridePPU = float.NaN)
        {
            var ppu = float.IsNaN(overridePPU) ? PPU : overridePPU;
            var pixels = units * ppu;
            return snapToPixel ? Vector2Round(pixels) : pixels;
        }
        
        public static float ToPixels(float units, bool snapToPixel = false)
        {
            var pixels = units * PPU;
            return snapToPixel ? Mathf.Round(pixels) : pixels;
        }

        public static Vector2 ToUnits(Vector2 pixels, bool snapToPixel = false)
        {
            pixels = snapToPixel ? Vector2Round(pixels) : pixels;
            var units = pixels / PPU;
            return units;
        }
        
        public static float ToUnits(float pixels, bool snapToPixel = false)
        {
            pixels = snapToPixel ? Mathf.Round(pixels) : pixels;
            var units = pixels / PPU;
            return units;
        }
        
        public static Vector2 SnapToPixels(Vector2 position, UnitType positionUnitType = UnitType.WorldUnits)
        {
            return positionUnitType == UnitType.Pixels ? 
                Vector2Round(position) : 
                ToUnits(ToPixels(position, true));
        }       
        
        public static Vector2 SnapXToPixels(Vector2 position, UnitType positionUnitType = UnitType.WorldUnits)
        {
            return new Vector2(SnapToPixels(position,positionUnitType).x, position.y);
        }
        
        public static Vector2 SnapYToPixels(Vector2 position, UnitType positionUnitType = UnitType.WorldUnits)
        {
            return new Vector2(position.x, SnapToPixels(position,positionUnitType).y);
        }
        
        public string ToString(Vector2 units, bool snapToPixel = false) 
        {
            return $"Pixels:{ToPixels(units, snapToPixel)} Units:{units}";
        }
        
    }
}
