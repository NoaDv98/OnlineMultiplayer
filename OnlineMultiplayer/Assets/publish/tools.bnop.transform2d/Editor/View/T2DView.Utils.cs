//----------------------------------------------
// Transform2D
// Copyright © 2024 BaconOppenheim™
//----------------------------------------------

using System;
using UnityEngine;

namespace tools.bnop.Transform2d.View
{
    public partial class T2DView
    {
        private static RectOffset UniformRectOffset(int offset)
        {
            return UniformRectOffset(offset, offset);
        }

        private static RectOffset UniformRectOffset(Vector2 offset)
        {
            return UniformRectOffset((int)offset.x, (int)offset.y);
        }

        private static RectOffset UniformRectOffset(int offsetX, int offsetY)
        {
            return new RectOffset(offsetX, offsetX, offsetY, offsetY);
        }

        private static GUIContent IconButtonContent(string icon, string tooltip)
        {
            return new GUIContent()
            {
                image = (Texture2D)Image(icon),
                tooltip = tooltip
            };
        }
        
        private static float CleanFloat(float value, int decimalPlaces = 3)
        {
            return Mathf.Round(value * Mathf.Pow(10f, decimalPlaces)) / Mathf.Pow(10f, decimalPlaces);
        }

        private static void DisableIf(bool condition, Action callback)
        {
            var prev = GUI.enabled;
            GUI.enabled = !condition;
            callback?.Invoke();
            GUI.enabled = prev;
        }
        
        private class Vector2Cache
        {
            public float? X;
            public float? Y;
        }
        
        private static Vector2 ConstrainProportions(Vector2 prevValue, Vector2 newValue, Vector2Cache cachedNonZero)
        {
            if (newValue == Vector2.zero)
            {
                cachedNonZero.X = null;
                cachedNonZero.Y = null;
                return newValue;
            }

            var width = newValue.x;
            var height = newValue.y;

            if (width != prevValue.x)
            {
                if (width == 0 && prevValue.x != 0)
                {
                    cachedNonZero.X = prevValue.x;
                }
                else
                {
                    var scalingFactor = cachedNonZero.X ?? prevValue.x;
                    if (scalingFactor != 0)
                    {
                        height *= (width / scalingFactor);
                    }

                    cachedNonZero.X = null;
                }
            }
            else if (height != prevValue.y)
            {
                if (height == 0 && prevValue.y != 0)
                {
                    cachedNonZero.Y = prevValue.y;
                }
                else
                {
                    var scalingFactor = cachedNonZero.Y ?? prevValue.y;
                    if (scalingFactor != 0)
                    {
                        width *= (height / scalingFactor);
                    }

                    cachedNonZero.Y = null;
                }
            }

            return new Vector2(width, height);
        }
    }
}
