//----------------------------------------------
// Transform2D
// Copyright © 2024 BaconOppenheim™
//----------------------------------------------

using System.Linq;
using tools.bnop.Transform2d.Service;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace tools.bnop.Transform2d.Data
{
    public class T2DColliderBounds : T2DObjectBounds<Collider2D>
    {
        
        public override Bounds ObjectAligned => Component? GetObjectAlignedBounds() : new Bounds();
        public override Bounds AxisAligned => Component ? Component.bounds : new Bounds();

        public T2DColliderBounds(GameObject gameObject) : base(gameObject) {}

        protected override bool ValidateComponent(Collider2D collider)
        {
            if (!collider) return false;
            if (collider is not (CompositeCollider2D or TilemapCollider2D or CustomCollider2D)) return true;
            T2DConsole.Log($"Detected not supported collider type: {collider.GetType()}");
            return false;
        }

        public override Vector2 GetVectorTo(Vector2 normalizedPoint, BoundsType boundsType = BoundsType.AlignObject, UnitSpace unitSpace = UnitSpace.Global)
        {
            if (!GameObject) return Vector2.zero;
            Physics2D.SyncTransforms();
            return base.GetVectorTo(normalizedPoint, boundsType, unitSpace);
        }
        
        private Bounds GetObjectAlignedBounds()
        {
            Vector2[] points;
            float left, right, bottom, top;
            Vector2 min, max, localCenter, localSize;
            
            switch (Component)
            {
                case BoxCollider2D box:
                    localCenter = box.offset;
                    localSize = new Vector2(box.size.x, box.size.y);
                    return new Bounds(localCenter, localSize);
                case CircleCollider2D circle:
                    localCenter = circle.offset;
                    localSize = new Vector2(circle.radius * 2, circle.radius * 2);
                    return new Bounds(localCenter, localSize);
                case CapsuleCollider2D capsule:
                    localSize = capsule.size;
                    localCenter = capsule.offset;
                    var adjustedSize = capsule.direction == CapsuleDirection2D.Horizontal ? new Vector2(localSize.y, localSize.x) :
                        localSize;
                    return new Bounds(localCenter, adjustedSize);
                case PolygonCollider2D polygon:
                    points = polygon.points;
                    left = points.Min(point => point.x);
                    right = points.Max(point => point.x);
                    bottom = points.Min(point => point.y);
                    top = points.Max(point => point.y);
                    min = new Vector2(left, bottom);
                    max = new Vector2(right, top);
                    localSize = max - min;
                    localCenter = min + localSize / 2f;
                    return new Bounds(localCenter, localSize);
                case EdgeCollider2D edge:
                    points = edge.points;
                    left = points.Min(point => point.x);
                    right = points.Max(point => point.x);
                    bottom = points.Min(point => point.y);
                    top = points.Max(point => point.y);
                    min = new Vector2(left, bottom);
                    max = new Vector2(right, top);
                    localSize = max - min;
                    localCenter = min + localSize / 2f;
                    return new Bounds(localCenter, localSize);
            }
            
            return new Bounds();
        }
        
    }
}
