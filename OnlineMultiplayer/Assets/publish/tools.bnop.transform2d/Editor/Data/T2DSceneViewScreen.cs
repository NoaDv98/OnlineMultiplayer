//----------------------------------------------
// Transform2D
// Copyright © 2024 BaconOppenheim™
//----------------------------------------------

using UnityEditor;
using UnityEngine;

namespace tools.bnop.Transform2d.Data
{
    public static class T2DSceneViewScreen
    {
        public static Bounds Bounds
        {
            get
            {
                var sceneView = SceneView.lastActiveSceneView;
                var camera = sceneView.camera;
                var bounds = new Bounds();
                bounds.SetMinMax(camera.ViewportToWorldPoint(new Vector3(0, 0, camera.nearClipPlane)),
                    camera.ViewportToWorldPoint(new Vector3(1, 1, camera.nearClipPlane)));
                return bounds;
            }
        }

        public static Rect ScreenRect => new(
            0,
            0,
            Screen.width / EditorGUIUtility.pixelsPerPoint,
            Screen.height / EditorGUIUtility.pixelsPerPoint
        );

        public static Vector2 ToScreenPosition(Vector3 worldPosition, UnitType unitType = UnitType.WorldUnits)
        {
            var sceneView = SceneView.lastActiveSceneView;
            if (unitType == UnitType.Pixels)
                worldPosition = T2DScreen.ToUnits(worldPosition);

            var screenPosition = (Vector2)sceneView.camera.WorldToScreenPoint(worldPosition) /
                                 EditorGUIUtility.pixelsPerPoint;
            var reversedScreenPosition = new Vector2(screenPosition.x,
                sceneView.camera.pixelHeight / EditorGUIUtility.pixelsPerPoint - screenPosition.y);
            return reversedScreenPosition;
        }

        public static float ToScreenCoord(Axis axis, float worldCoord, UnitType unitType = UnitType.WorldUnits,
            bool reverseAxis = false)
        {
            if (reverseAxis)
                return ToScreenCoord(AxisUtils.Opposite(axis), worldCoord, unitType);
            var worldPosition = AxisUtils.IsHorizontal(axis)
                ? new Vector3(worldCoord, 0, 0)
                : new Vector3(0, worldCoord, 0);
            var screenPosition = ToScreenPosition(worldPosition, unitType);
            return AxisUtils.CoordFromVec2(screenPosition, axis);
        }

        public static float ToScreenCoord(T2DGuide guide)
        {
            return ToScreenCoord(guide.Axis, guide.Position, reverseAxis: true);
        }

        public static Vector2 ToWorldPosition(Vector2 screenPosition)
        {
            var sceneView = SceneView.lastActiveSceneView;
            var reversed = new Vector2(screenPosition.x,
                sceneView.camera.pixelHeight / EditorGUIUtility.pixelsPerPoint - screenPosition.y);
            var scaled = reversed * EditorGUIUtility.pixelsPerPoint;
            return sceneView.camera.ScreenToWorldPoint(scaled);
        }

        public static float ToWorldCoord(Axis axis, float screenCoord, bool reverseAxis = false)
        {
            if (reverseAxis)
                return ToWorldCoord(AxisUtils.Opposite(axis), screenCoord);
            var screenPosition =
                AxisUtils.IsHorizontal(axis) ? new Vector2(screenCoord, 0) : new Vector2(0, screenCoord);
            var worldPosition = ToWorldPosition(screenPosition);
            return AxisUtils.CoordFromVec2(worldPosition, axis);
        }
    }
}
