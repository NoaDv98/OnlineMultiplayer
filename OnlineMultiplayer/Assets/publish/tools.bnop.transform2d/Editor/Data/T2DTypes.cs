//----------------------------------------------
// Transform2D
// Copyright © 2024 BaconOppenheim™
//----------------------------------------------

using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;

namespace tools.bnop.Transform2d.Data
{
    public enum UnitType
    {
        Pixels = 0,
        WorldUnits = 1
    };

    public enum UnitSpace
    {
        Local = 0,
        Global = 1
    }

    public enum BoundsSource
    {
        Camera,
        Collider,
        Sprite,
        Text,
        None
    }

    public enum BoundsType
    {
        AlignObject,
        AlignAxis
    }

    public enum PivotType
    {
        [Description(T2DTexts.PivotCenter)]
        Center,
        [Description(T2DTexts.PivotTopLeft)] 
        TopLeft,
        [Description(T2DTexts.PivotTopCenter)] 
        TopCenter,
        [Description(T2DTexts.PivotTopRight)] 
        TopRight,
        [Description(T2DTexts.PivotLeftCenter)] 
        LeftCenter,
        [Description(T2DTexts.PivotRightCenter)] 
        RightCenter,
        [Description(T2DTexts.PivotBottomLeft)] 
        BottomLeft,
        [Description(T2DTexts.PivotBottomCenter)] 
        BottomCenter,
        [Description(T2DTexts.PivotBottomRight)] 
        BottomRight
    }

    public static class Pivot
    {
        private static readonly Dictionary<PivotType, Vector2> Values = new()
        {
            [PivotType.Center] = new Vector2(0.5f, 0.5f),
            [PivotType.TopLeft] = new Vector2(0f, 1f),
            [PivotType.TopCenter] = new Vector2(0.5f, 1f),
            [PivotType.TopRight] = new Vector2(1f, 1f),
            [PivotType.LeftCenter] = new Vector2(0f, 0.5f),
            [PivotType.RightCenter] = new Vector2(1f, 0.5f),
            [PivotType.BottomLeft] = new Vector2(0f, 0f),
            [PivotType.BottomCenter] = new Vector2(0.5f, 0f),
            [PivotType.BottomRight] = new Vector2(1f, 0f)
        };

        public static Vector2 ValueOf(PivotType type) => Values[type];
        
        public static string NameOf(PivotType type)
        {
            return type.GetType()
                .GetField(type.ToString())
                ?.GetCustomAttribute<DescriptionAttribute>()
                ?.Description ?? type.ToString();
        }

        public static Vector2 Center => Values[PivotType.Center];
        public static Vector2 TopLeft => Values[PivotType.TopLeft];
        public static Vector2 TopCenter => Values[PivotType.TopCenter];
        public static Vector2 TopRight => Values[PivotType.TopRight];
        public static Vector2 LeftCenter => Values[PivotType.LeftCenter];
        public static Vector2 RightCenter => Values[PivotType.RightCenter];
        public static Vector2 BottomLeft => Values[PivotType.BottomLeft];
        public static Vector2 BottomCenter => Values[PivotType.BottomCenter];
        public static Vector2 BottomRight => Values[PivotType.BottomRight];
    }

    public enum Axis
    {
        Horizontal,
        Vertical
    }

    public static class AxisUtils
    {
        public static bool IsHorizontal(Axis axis) => axis == Axis.Horizontal;
        public static bool IsVertical(Axis axis) => axis == Axis.Vertical;
        public static Axis Opposite(Axis axis) => IsHorizontal(axis) ? Axis.Vertical : Axis.Horizontal;
        public static float CoordFromVec2(Vector2 vec2, Axis axis) => AxisUtils.IsHorizontal(axis) ? vec2.x : vec2.y;

    }
    
    public enum AlignOptions
    {
        Left,
        HorizontalCenter,
        Right,
        Top,
        VerticalCenter,
        Bottom
    }

    public enum DistributeOptions
    {
        Left,
        HorizontalCenters,
        Right,
        HorizontalSpacing,
        Top,
        VerticalSpacing,
        Bottom,
        VerticalCenters
    }

    public struct DistributeData
    {
        public DistributeOptions Option;
        public float Space;
    }
}
