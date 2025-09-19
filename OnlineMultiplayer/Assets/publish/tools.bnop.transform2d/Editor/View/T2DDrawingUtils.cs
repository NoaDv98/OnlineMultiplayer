//----------------------------------------------
// Transform2D
// Copyright © 2024 BaconOppenheim™
//----------------------------------------------

using tools.bnop.Transform2d.Data;
using UnityEngine;

namespace tools.bnop.Transform2d.View
{
    public class T2DDrawingUtils
    {
        public static void DrawRectangle(Vector2 position, Vector2 size, Color color)
        {
            var rect = new Rect(position.x, position.y, size.x, size.y);
            var prevColor = GUI.color;
            GUI.color = color;
            GUI.DrawTexture(rect, Texture2D.whiteTexture);
            GUI.color = prevColor;
        }

        public static void DrawLine(Axis axis, float position, float offset, float length, Color color)
        {
            var width = axis == Axis.Horizontal ? length : 1;
            var height = axis == Axis.Horizontal ? 1 : length;
            var rect = new Rect(axis == Axis.Horizontal ? offset : position,
                axis == Axis.Horizontal ? position : offset, width,
                height);
            var prevColor = GUI.color;
            GUI.color = color;
            GUI.DrawTexture(rect, Texture2D.whiteTexture);
            GUI.color = prevColor;
        }
    }
}
