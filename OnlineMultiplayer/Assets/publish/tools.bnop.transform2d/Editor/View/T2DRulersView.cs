//----------------------------------------------
// Transform2D
// Copyright © 2024 BaconOppenheim™
//----------------------------------------------

using System;
using System.Linq;
using tools.bnop.Transform2d.Data;
using UnityEngine;

namespace tools.bnop.Transform2d.View
{
    public class T2DRulersView
    {
        private bool _isFirstRender = true;
        private readonly T2DRuler _horizontalRuler = new(Axis.Horizontal);
        private readonly T2DRuler _verticalRuler = new(Axis.Vertical);
        private UnitType _unitType;
        private Bounds? _selectionBounds;

        public UnitType UnitType
        {
            set => _unitType = value;
        }
        
        internal Bounds? SelectionBounds
        {
            set => _selectionBounds = value;
        }

        public void Render()
        {
            if (_isFirstRender)
            {
                _isFirstRender = false;
                return;
            }

            T2DDrawingUtils.DrawRectangle(Vector2.zero, Vector2.one * T2DRuler.RulerThickness, T2DRuler.BgColor);

            _horizontalRuler.UnitType = _unitType;
            _horizontalRuler.SelectionBounds = _selectionBounds;
            _horizontalRuler.Render();

            _verticalRuler.UnitType = _unitType;
            _verticalRuler.SelectionBounds = _selectionBounds;
            _verticalRuler.Render();
        }

        internal T2DRuler RulerUnderMouse => _horizontalRuler.IsUnderMouse ? _horizontalRuler :
            _verticalRuler.IsUnderMouse ? _verticalRuler : null;

        internal static bool IsMouseOverCorner()
        {
            var rect = new Rect(Vector2.zero, Vector2.one * T2DRuler.RulerThickness);
            return rect.Contains(Event.current.mousePosition);
        }
    }

    internal class T2DRuler
    {
        internal const float RulerThickness = 22;
        internal static readonly Color BgColor = new(.15f, .15f, .15f);
        private static readonly Color BorderColor = new(1, 1, 1, 0.3f);
        private static readonly Color TickColor = new(1, 1, 1, 0.5f);
        private static readonly Color LabelColor = new(1, 1, 1, 0.7f);
        private static readonly Color SelectionBoundsColor = new(70f / 255f, 96f / 255f, 125f / 255f, 0.5f);

        private static readonly float[] TickIntervalsUnits =
        {
            0.025f, 0.05f, 0.1f, 0.25f, 0.5f, 1, 2.5f, 5, 10, 25, 50, 100, 250, 500, 1000, 2500, 5000, 10000, 25000,
            50000, 100000, 250000, 500000, 1000000
        };

        private static readonly float[] TickIntervalsPixels =
        {
            1f, 5, 10, 25, 50, 100, 250, 500, 1000, 2500, 5000, 10000, 25000, 50000, 100000, 250000, 500000, 1000000
        };

        private Axis _axis;
        private UnitType _unitType;
        private Bounds? _selectionBounds;
        
        private float Length => _axis == Axis.Horizontal ? GetRulerRect().width : GetRulerRect().height - RulerThickness;

        private static readonly GUIStyle LabelStyle = new()
        {
            fontSize = 10,
            normal = { textColor = LabelColor },
            hover = { textColor = LabelColor },
            focused = { textColor = LabelColor },
            alignment = TextAnchor.MiddleCenter,
            padding = new RectOffset(4, 4, 4, 4),
        };

        public T2DRuler(Axis axis)
        {
            _axis = axis;
        }

        internal UnitType UnitType
        {
            set => _unitType = value;
        }

        internal Bounds? SelectionBounds
        {
            set => _selectionBounds = value;
        }

        internal Axis Axis => _axis;

        internal void Render()
        {
            DrawBg();
            DrawSelectionBounds();
            DrawTicks();
        }

        private void DrawBg()
        {
            var rect = GetRulerRect();
            T2DDrawingUtils.DrawRectangle(rect.min, rect.size, BgColor);
            T2DDrawingUtils.DrawLine(_axis, RulerThickness, RulerThickness, Length,
                BorderColor);
        }

        private void DrawTicks()
        {
            float minWorldCoord, maxWorldCoord;
            switch (_axis)
            {
                case Axis.Horizontal:
                    minWorldCoord = T2DSceneViewScreen.ToWorldCoord(_axis, RulerThickness);
                    maxWorldCoord = T2DSceneViewScreen.ToWorldCoord(_axis, T2DSceneViewScreen.ScreenRect.width);
                    break;
                case Axis.Vertical:
                    minWorldCoord = T2DSceneViewScreen.ToWorldCoord(_axis, T2DSceneViewScreen.ScreenRect.height);
                    maxWorldCoord = T2DSceneViewScreen.ToWorldCoord(_axis, RulerThickness);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_axis), _axis, null);
            }

            if (_unitType == UnitType.Pixels)
            {
                minWorldCoord = T2DScreen.ToPixels(minWorldCoord);
                maxWorldCoord = T2DScreen.ToPixels(maxWorldCoord);
            }

            var worldLength = maxWorldCoord - minWorldCoord;
            var worldScreenRatio = worldLength / Length;
            var rawInterval = 120 * worldScreenRatio;


            var tickIntervals = _unitType == UnitType.Pixels ? TickIntervalsPixels : TickIntervalsUnits;
            var tickInterval = tickIntervals.LastOrDefault(t => t <= rawInterval);
            if (tickInterval == 0) tickInterval = tickIntervals[0];

            var firstTick = Mathf.Ceil(minWorldCoord / tickInterval) * tickInterval;

            for (var currentTick = firstTick; currentTick <= maxWorldCoord; currentTick += tickInterval)
            {
                DrawTick(currentTick);
                DrawLabel(currentTick);
            }
        }

        private void DrawTick(float worldCoord)
        {
            var tickAxis = _axis == Axis.Horizontal ? Axis.Vertical : Axis.Horizontal;
            var coord = T2DSceneViewScreen.ToScreenCoord(_axis, worldCoord, _unitType);
            T2DDrawingUtils.DrawLine(tickAxis, coord, RulerThickness * .8f,
                RulerThickness * .2f,
                TickColor);
        }

        private void DrawMousePosition()
        {
            var tickAxis = _axis == Axis.Horizontal ? Axis.Vertical : Axis.Horizontal;
            var mousePosition = _axis == Axis.Horizontal ? Event.current.mousePosition.x : Event.current.mousePosition.y;
            T2DDrawingUtils.DrawLine(tickAxis, mousePosition, RulerThickness * .6f,
                RulerThickness * .4f, LabelColor);
        }

        private void DrawSelectionBounds()
        {
            if (_selectionBounds == null) return;

            var worldMin = _axis == Axis.Horizontal ? _selectionBounds.Value.min.x : _selectionBounds.Value.min.y;
            var worldMax = _axis == Axis.Horizontal ? _selectionBounds.Value.max.x : _selectionBounds.Value.max.y;
            var screenMin = T2DSceneViewScreen.ToScreenCoord(_axis, worldMin);
            var screenMax = T2DSceneViewScreen.ToScreenCoord(_axis, worldMax);
            var length = screenMax - screenMin;

            var position = new Vector2(
                _axis == Axis.Horizontal ? screenMin : 0,
                _axis == Axis.Horizontal ? 0 : screenMin
            );

            var size = new Vector2(
                _axis == Axis.Horizontal ? length : RulerThickness,
                _axis == Axis.Horizontal ? RulerThickness : length
            );

            T2DDrawingUtils.DrawRectangle(position, size, SelectionBoundsColor);
        }

        private void DrawLabel(float worldCoord)
        {
            var coord = T2DSceneViewScreen.ToScreenCoord(_axis, worldCoord, _unitType);
            const float width = 60;
            const float height = RulerThickness * .8f;
            var position = _axis == Axis.Horizontal
                ? new Vector2(coord - width / 2, 0)
                : new Vector2(0, coord + width / 2);
            var size = new Vector2(width, height);

            var text = CoordToLabelString(worldCoord);

            if (_axis == Axis.Horizontal)
                GUI.Label(new Rect(position, size), text, LabelStyle);
            else
            {
                
                var matrixBackup = GUI.matrix;
                GUIUtility.RotateAroundPivot(-90, Vector2.zero);
                GUI.matrix = Matrix4x4.Translate(new Vector3(position.x, position.y, 0)) * GUI.matrix;
                GUI.Label(new Rect(Vector2.zero, size), text, LabelStyle);
                GUI.matrix = matrixBackup;
            }
        }

        private static float CleanFloat(float value, int decimalPlaces = 3)
        {
            return Mathf.Round(value * Mathf.Pow(10f, decimalPlaces)) / Mathf.Pow(10f, decimalPlaces);
        }

        private static string CoordToLabelString(float coord)
        {
            coord = CleanFloat(coord);
            var isPositive = coord >= 0;
            var absCoord = Mathf.Abs(coord);
            return $@"{(isPositive ? "" : "-")}{absCoord switch
                {
                    >= 1000000 => $"{absCoord / 1000000}M",
                    >= 10000 and < 1000000 => $"{absCoord / 1000}K",
                    _ => $"{absCoord}"
                }
            }";
        }

        private Rect GetRulerRect()
        {
            var position = _axis == Axis.Horizontal ? new Vector2(RulerThickness, 0) : new Vector2(0, RulerThickness);
            var size = _axis == Axis.Horizontal
                ? new Vector2(T2DSceneViewScreen.ScreenRect.width - RulerThickness, RulerThickness)
                : new Vector2(RulerThickness, T2DSceneViewScreen.ScreenRect.height - RulerThickness);

            return new Rect(position, size);
        }

        internal bool IsUnderMouse
        {
            get
            {
                var rect = GetRulerRect();
                return rect.Contains(Event.current.mousePosition);
            }
        }
    }
}
