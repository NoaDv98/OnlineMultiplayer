//----------------------------------------------
// Transform2D
// Copyright © 2024 BaconOppenheim™
//----------------------------------------------

using UnityEngine;
using UnityEditor;


namespace tools.bnop.Transform2d.Data
{

    public struct T2DPositionUpdateAction
    {
        public readonly T2DSelection Target;
        public Vector2 Anchor { get; private set; }
        public Vector2 Position { get; set; }

        
        public float X
        {
            get => Position.x;
            set => Position = new Vector2(value, Position.y);
        }
        
        public float Y
        {
            get => Position.y;
            set => Position = new Vector2(Position.x, value);
        }
        
        public float AnchorX
        {
            get => Anchor.x;
            set => Anchor = new Vector2(value, Anchor.y);
        }
        
        public float AnchorY
        {
            get => Anchor.y;
            set => Anchor = new Vector2(Anchor.x, value);
        }
        
        public T2DPositionUpdateAction(T2DSelection target, Vector2 position, Vector2 anchor)
        {
            Target = target;
            Position = position;
            Anchor = anchor;
        }
    } 
    
    public static class T2DSnapping
    {
        
        private static float _force = 0.1f;

        public static T2DPositionUpdateAction SnapPosition(T2DSelection target)
        {
            var sceneViewZoom = SceneView.lastActiveSceneView.size;
            _force = T2DSettingsData.SnapToGuidesForce * sceneViewZoom * 0.5f;
            
            var anchor = target.First.Bounds.GetAABBVectorTo(target.Position);
            var snapAction = new T2DPositionUpdateAction(target, target.GetPosition(anchor), anchor);

            bool isSnapHorizontal = false, isSnapVertical = false;
            var selectionBounds = target.Bounds;

            if (Transform2D.Model.Settings.SnapToGuides)
            {
                isSnapHorizontal = T2DGuides.IsSnap(selectionBounds, _force, Axis.Horizontal);
                isSnapVertical = T2DGuides.IsSnap(selectionBounds, _force, Axis.Vertical);
            }
            
            switch (isSnapHorizontal)
            {
                case false when !isSnapVertical:
                    return Transform2D.Model.Settings.SnapToPixel ? SnapPixels(snapAction) : snapAction;
                
                case true when !isSnapVertical:
                {
                    snapAction = SnapHorizontalGuides(snapAction);
                    if (Transform2D.Model.Settings.SnapToPixel)
                        snapAction.Position = T2DScreen.SnapXToPixels(snapAction.Position);
                    return snapAction;
                }
                case false: 
                {
                    snapAction = SnapVerticalGuides(snapAction);
                    if (Transform2D.Model.Settings.SnapToPixel)
                        snapAction.Position = T2DScreen.SnapYToPixels(snapAction.Position);
                    return snapAction;
                }
            }

            var snapHorizontal = SnapHorizontalGuides(snapAction);
            var snapVertical = SnapVerticalGuides(snapAction);
            snapAction = snapHorizontal;
            snapAction.X = snapVertical.Position.x;
            snapAction.AnchorX = snapVertical.Anchor.x;
            return snapAction;
        }
        
        private static T2DPositionUpdateAction SnapPixels(T2DPositionUpdateAction snapAction)
        {
            snapAction.Position = T2DScreen.SnapToPixels(snapAction.Position);
            return snapAction;
        }

        private static T2DPositionUpdateAction SnapHorizontalGuides(T2DPositionUpdateAction snapAction)
        {
            var target = snapAction.Target;
            
            var guideTop = T2DGuides.GetGuideFor(target.Top, _force, Axis.Horizontal);
            var guideBottom = T2DGuides.GetGuideFor(target.Bottom, _force, Axis.Horizontal);
            var guideCenter = T2DGuides.GetGuideFor(target.VerticalCenter, _force, Axis.Horizontal);
            
            if(guideTop is null && guideBottom is null && guideCenter is null)
                return snapAction;
            
            var guideTopDistance = guideTop != null ? Mathf.Abs(guideTop.Position - target.Top) : float.MaxValue;
            var guideBottomDistance = guideBottom != null ? Mathf.Abs(guideBottom.Position - target.Bottom) : float.MaxValue;
            var guideCenterDistance = guideCenter != null ? Mathf.Abs(guideCenter.Position - target.VerticalCenter) : float.MaxValue;

            var minDistance = Mathf.Min(guideTopDistance, guideBottomDistance, guideCenterDistance);
            if (guideTop != null && Mathf.Approximately(minDistance, guideTopDistance))
            {
                snapAction.Y = guideTop.Position;
                snapAction.AnchorY = Pivot.TopCenter.y;
            }
            else if (guideBottom != null && Mathf.Approximately(minDistance, guideBottomDistance))
            {
                snapAction.Y = guideBottom.Position;
                snapAction.AnchorY = Pivot.BottomCenter.y;
            }
            else if (guideCenter != null && Mathf.Approximately(minDistance, guideCenterDistance))
            {
                snapAction.Y = guideCenter.Position;
                snapAction.AnchorY = Pivot.Center.y;
            }

            return snapAction;
        }
        
        private static T2DPositionUpdateAction SnapVerticalGuides(T2DPositionUpdateAction snapAction)
        {
            var target = snapAction.Target;
            
            var guideLeft = T2DGuides.GetGuideFor(target.Left, _force, Axis.Vertical);
            var guideRight = T2DGuides.GetGuideFor(target.Right, _force, Axis.Vertical);
            var guideCenter = T2DGuides.GetGuideFor(target.HorizontalCenter, _force, Axis.Vertical);
            
            if(guideLeft is null && guideRight is null && guideCenter is null)
                return snapAction;
            
            var guideLeftDistance = guideLeft != null ? Mathf.Abs(guideLeft.Position - target.Left) : float.MaxValue;
            var guideRightDistance = guideRight != null ? Mathf.Abs(guideRight.Position - target.Right) : float.MaxValue;
            var guideCenterDistance = guideCenter != null ? Mathf.Abs(guideCenter.Position - target.HorizontalCenter) : float.MaxValue;

            var minDistance = Mathf.Min(guideLeftDistance, guideRightDistance, guideCenterDistance);
            if (guideLeft != null && Mathf.Approximately(minDistance, guideLeftDistance))
            {
                snapAction.X = guideLeft.Position;
                snapAction.AnchorX = Pivot.LeftCenter.x;
            }
            else if (guideRight != null && Mathf.Approximately(minDistance, guideRightDistance))
            {
                snapAction.X = guideRight.Position;
                snapAction.AnchorX = Pivot.RightCenter.x;
            }
            else if (guideCenter != null && Mathf.Approximately(minDistance, guideCenterDistance))
            {
                snapAction.X = guideCenter.Position;
                snapAction.AnchorX = Pivot.Center.x;
            }

            return snapAction;
        }
        
    }
}
