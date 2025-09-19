//----------------------------------------------
// Transform2D
// Copyright © 2024 BaconOppenheim™
//----------------------------------------------

using UnityEngine;

namespace tools.bnop.Transform2d.Data
{

    public interface IObjectBounds<out T>
    {
        public T Component { get; }
        
        public Bounds ObjectAligned { get; }
        public Bounds AxisAligned { get; }
        
        public Vector2 GetVectorTo(Vector2 normalizedPoint, BoundsType boundsType = BoundsType.AlignObject, UnitSpace unitSpace = UnitSpace.Global);
        public Vector2 GetAABBVectorTo(Vector2 worldPoint, bool normalizeToBounds = true);

        public Vector2 TopLeft { get; }
        public Vector2 TopRight { get; }
        public Vector2 BottomLeft { get; }
        public Vector2 BottomRight { get; }
        public Vector2 Center { get; }
        public Vector2 LeftCenter { get; }
        public Vector2 RightCenter { get; }
        public Vector2 TopCenter { get; }
        public Vector2 BottomCenter { get; }

        public float Width { get; }
        public float Height { get; }
        public Vector2 Size { get; }
    }
    
    public abstract class T2DObjectBounds<T> : IObjectBounds<T>
    {
        
        protected readonly GameObject GameObject;
        protected Transform Transform => GameObject?.transform;

        private T _cacheComponent;

        private Bounds _cacheObjectAligned;
        private Bounds _cacheAxisAligned;

        public virtual Bounds ObjectAligned { get; protected set; } = new Bounds();

        public virtual Bounds AxisAligned => new (Transform.TransformPoint(Vector3.zero), Vector3.zero);

        protected T2DObjectBounds(GameObject gameObject)
        {
            GameObject = gameObject;
        }
        
        public virtual T Component
        {
            get
            {
                _cacheComponent ??= GameObject.GetComponent<T>();
                return ValidateComponent(_cacheComponent) ? _cacheComponent : default;
            }
        }

        protected virtual bool ValidateComponent(T component)
        {
            return true;
        }
        
        public void ResetComponent()
        {
            _cacheComponent = default;
        }
        
        public virtual Vector2 GetVectorTo(Vector2 normalizedPoint, BoundsType boundsType = BoundsType.AlignObject, UnitSpace unitSpace = UnitSpace.Global)
        {
            if (!GameObject) return Vector3.zero;

            var size = boundsType == BoundsType.AlignObject ? ObjectAligned.size : AxisAligned.size;
            var bottomLeft = boundsType == BoundsType.AlignObject ? ObjectAligned.min : AxisAligned.min;
            var targetVector = size * normalizedPoint + (Vector2)bottomLeft;

            if (boundsType == BoundsType.AlignAxis)
            {
                targetVector -= (Vector2)Transform.position;
            }
            else if (unitSpace == UnitSpace.Global)
            {
                targetVector = Transform.TransformVector(targetVector);
            }
            else
            {
                targetVector = TransformVectorToParent(targetVector);
            }
            
            return targetVector;
        }

        public Vector2 GetAABBVectorTo(Vector2 worldPoint, bool normalizeToBounds = true)
        {
            var bounds = AxisAligned;
            var bottomLeftCorner = bounds.min;
            var vectorToTarget = worldPoint - (Vector2)bottomLeftCorner;
            
            return normalizeToBounds ? NormalizeVectorToBounds(vectorToTarget, bounds) : vectorToTarget;
        }

        private static Vector2 NormalizeVectorToBounds(Vector2 vector, Bounds bounds)
        {
            var sizeX = bounds.size.x == 0 ? 1 : bounds.size.x;
            var sizeY = bounds.size.y == 0 ? 1 : bounds.size.y;
            
            var normalizedVector = new Vector2(
                vector.x / sizeX,
                vector.y / sizeY
            );

            return normalizedVector;
        }
        
        private Vector2 TransformVectorToParent(Vector2 target)
        {
            var v = Vector2.Scale(target, Transform.localScale);
            return Transform.localRotation * v;
            
        }        

        public Vector2 TopLeft => GetVectorTo(Pivot.TopLeft);
        public Vector2 TopRight => GetVectorTo(Pivot.TopRight);
        public Vector2 BottomLeft => GetVectorTo(Pivot.BottomLeft);
        public Vector2 BottomRight => GetVectorTo(Pivot.BottomRight);
        public Vector2 Center => GetVectorTo(Pivot.Center);
        public Vector2 LeftCenter => GetVectorTo(Pivot.LeftCenter);
        public Vector2 RightCenter => GetVectorTo(Pivot.RightCenter);
        public Vector2 TopCenter => GetVectorTo(Pivot.TopCenter);
        public Vector2 BottomCenter => GetVectorTo(Pivot.BottomCenter);

        public float Width => Vector2.Distance(BottomRight, BottomLeft);
        public float Height => Vector2.Distance(TopLeft, BottomLeft);
        public Vector2 Size => new Vector2(Width, Height);

    }
}
