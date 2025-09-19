//----------------------------------------------
// Transform2D
// Copyright © 2024 BaconOppenheim™
//----------------------------------------------

using TMPro;
using tools.bnop.Transform2d.Service;
using UnityEngine;

namespace tools.bnop.Transform2d.Data
{
    
    public class T2DGameObject
    {
        
        private GameObject _target;
        private BoundsSource _boundsSource = BoundsSource.None;

        private T2DColliderBounds ColliderBounds { get; set; }
        private T2DSpriteBounds SpriteBounds { get; set; }
        private T2DCameraBounds CameraBounds { get; set; }
        private T2DTextBounds TextBounds { get; set; }

        public IObjectBounds<object> Bounds { get; private set; } 
        public Vector2 Pivot { get; set; } = Data.Pivot.Center;
        public SpriteRenderer Renderer => SpriteBounds.Component;
        public Collider2D Collider => ColliderBounds.Component;
        public Camera Camera => CameraBounds.Component;
        public TextMeshPro Text => TextBounds.Component;
        public Transform Transform => Target.transform;
        
        public float SpritePPU => SpriteBounds.SpritePPU;           
        public float SpritePPUSetting { get; private set; }         
        public float SpriteScale => SpritePPU / SpritePPUSetting;   
        public bool IsSpriteDownscaled => SpriteScale < 1;
        public Vector2 SpritePivot => Vector2.Scale(SpriteBounds.SpritePivot, Transform.localScale);

        
        public T2DGameObject(GameObject gameObject)
        {
            Target = gameObject;
        }

        public BoundsSource BoundsSource
        {
            get => _boundsSource;
            set
            {
                var renderer = Renderer is not null;
                var collider = Collider is not null;
                var camera = Camera is not null;
                var text = Text is not null;

                _boundsSource = value switch
                {
                    BoundsSource.Sprite => renderer ? BoundsSource.Sprite : collider ? BoundsSource.Collider : BoundsSource.None,
                    BoundsSource.Collider => collider ? BoundsSource.Collider : renderer ? BoundsSource.Sprite : BoundsSource.None,
                    _ => BoundsSource.None
                };

                if (text) _boundsSource = BoundsSource.Text;
                if (camera) _boundsSource = BoundsSource.Camera;
                
                Bounds = BoundsSource switch
                {
                    BoundsSource.Collider => ColliderBounds, 
                    BoundsSource.Sprite => SpriteBounds,
                    BoundsSource.Text => TextBounds,
                    BoundsSource.Camera => CameraBounds,
                    _ => new T2DNoBounds(Target)
                };
            } 
        }

        public BoundsSource SetBoundsSource(BoundsSource source)
        {
            BoundsSource = source;
            return BoundsSource;
        }
        
        public GameObject Target
        {
            get => _target;
            private set
            {
                _target = value;
                
                SpriteBounds = new T2DSpriteBounds(_target);
                ColliderBounds = new T2DColliderBounds(_target);
                CameraBounds = new T2DCameraBounds(_target);
                TextBounds = new T2DTextBounds(_target);

                BoundsSource = BoundsSource.None;

                if (Text) BoundsSource = BoundsSource.Text;
                if (Camera) BoundsSource = BoundsSource.Camera;
                
                SpritePPUSetting = T2DDSpriteAsset.GetSpritePPUSetting(_target);
            }
        }

        public Vector2 LocalScale
        {
            get => Transform.localScale;
            set
            {
                var position = Position;
                Transform.localScale = new Vector3(value.x, value.y, Transform.localScale.z);
                Position = position; 
            }
        }

        private static Vector3 GetParentScale(Transform transform)
        {
            var originalScale = transform.localScale;
            transform.localScale = Vector3.one;
            var parentScale = transform.lossyScale;
            transform.localScale = originalScale;
    
            return parentScale;
        }
        
        public Vector2 Scale
        {
            get => Transform.lossyScale;
            set
            {
                var position = Position;
                var parentScale = GetParentScale(Transform);
                var targetLocalScale = new Vector3(
                    value.x / parentScale.x,
                    value.y / parentScale.y,
                    Transform.localScale.z
                );
                Transform.localScale = targetLocalScale;
                Position = position; 
            }
        }
        
        public Vector2 Size
        {
            get => Bounds.Size;
            set
            {
                var position = Position;
                Transform.localScale = new Vector3(1, 1, Transform.localScale.z);
                var fullSize = Bounds.Size;
                var targetLocalScale = new Vector3(
                    fullSize.x != 0 ? (value.x / fullSize.x)  : 0,
                    fullSize.y != 0 ? (value.y / fullSize.y)  : 0,
                    Transform.localScale.z
                );  
                Transform.localScale = targetLocalScale;
                Position = position; 
            }
        }
        
        public Vector2 LocalSize
        {
            get => Vector3.Scale(Bounds.ObjectAligned.size, Transform.localScale);
            set
            {
                var position = Position;
                var localScale = Transform.localScale;
                var currentSize = Bounds.ObjectAligned.size;
                var targetLocalScale = new Vector3(
                    currentSize.x != 0 ? value.x / currentSize.x : 0,
                    currentSize.y != 0 ? value.y / currentSize.y : 0,
                    localScale.z
                );  
                Transform.localScale = targetLocalScale;
                Position = position; 
            }
        }
        
        private static Vector2 RotateVector2(Vector2 vector, float angleDegrees)
        {
            var angleRadians = angleDegrees * Mathf.Deg2Rad;
            
            var cos = Mathf.Cos(angleRadians);
            var sin = Mathf.Sin(angleRadians);
            var newX = vector.x * cos - vector.y * sin;
            var newY = vector.x * sin + vector.y * cos;
            
            return new Vector2(newX, newY);
        }
        
        public float Rotation
        {
            get => Transform.rotation.eulerAngles.z;
            set
            {
                var currentOffset = -Bounds.GetVectorTo(Pivot, BoundsType.AlignObject, UnitSpace.Global);
            
                var originalOffset = RotateVector2(currentOffset, -Rotation);
                var newOffset = RotateVector2(originalOffset, value);
            
                var rotatedPosition = Position + newOffset;
                Transform.position = new Vector3(rotatedPosition.x, rotatedPosition.y, Transform.position.z);
                Transform.eulerAngles = new Vector3(0, 0, value);
            }
        }

        public float LocalRotation
        {
            get => Transform.localRotation.eulerAngles.z;
            set
            {
                var currentOffset = -Bounds.GetVectorTo(Pivot, BoundsType.AlignObject, UnitSpace.Local);
            
                var originalOffset = RotateVector2(currentOffset, -LocalRotation);
                var newOffset = RotateVector2(originalOffset, value);
            
                var rotatedPosition = LocalPosition + newOffset;
                Transform.localPosition = new Vector3(rotatedPosition.x, rotatedPosition.y, Transform.localPosition.z);
                Transform.localEulerAngles = new Vector3(0, 0, value);
            }
        }
        
        public void SetPosition(Vector2 position, Vector2 pivot, BoundsType boundsType = BoundsType.AlignObject, UnitSpace unitSpace = UnitSpace.Global)
        {
            var pivotOffset = Bounds.GetVectorTo(pivot, boundsType, unitSpace);
            position -= pivotOffset;
            
            if (unitSpace == UnitSpace.Global)
                Transform.position = new Vector3(position.x, position.y, Transform.position.z);
            else
                Transform.localPosition = new Vector3(position.x, position.y,Transform.localPosition.z);
            
            Transform.hasChanged = false;
        }

        public Vector2 GetPosition(Vector2 pivot, BoundsType boundsType = BoundsType.AlignObject, UnitSpace unitSpace = UnitSpace.Global)
        {
            var position = unitSpace == UnitSpace.Global
                ? new Vector2(Transform.position.x, Transform.position.y)
                : new Vector2(Transform.localPosition.x, Transform.localPosition.y);
            
            var pivotOffset = Bounds.GetVectorTo(pivot, boundsType, unitSpace);
            position += pivotOffset;

            return position;
        }
        
        public Vector2 Position
        {
            get => GetPosition(Pivot);
            set => SetPosition(value, Pivot);
        }        
        
        public Vector2 LocalPosition
        {
            get => GetPosition(Pivot, BoundsType.AlignObject, UnitSpace.Local);
            set => SetPosition(value, Pivot, BoundsType.AlignObject, UnitSpace.Local);
        }
        
        public Vector2 Center
        {
            get => GetPosition(Data.Pivot.Center, BoundsType.AlignAxis);
            set => SetPosition(value, Data.Pivot.Center, BoundsType.AlignAxis);
        }

        public float HorizontalCenter
        {
            get => GetPosition(Data.Pivot.Center, BoundsType.AlignAxis).x;
            set
            {
                var position = GetPosition(Data.Pivot.Center, BoundsType.AlignAxis);
                position.x = value;
                SetPosition(position, Data.Pivot.Center, BoundsType.AlignAxis);
            }
        }

        public float VerticalCenter
        {
            get => GetPosition(Data.Pivot.Center, BoundsType.AlignAxis).y;
            set
            {
                var position = GetPosition(Data.Pivot.Center, BoundsType.AlignAxis);
                position.y = value;
                SetPosition(position, Data.Pivot.Center, BoundsType.AlignAxis);
            }
        }

        public float Left
        {
            get => GetPosition(Data.Pivot.TopLeft, BoundsType.AlignAxis).x;
            set
            {
                var position = GetPosition(Data.Pivot.TopLeft, BoundsType.AlignAxis);
                position.x = value;
                SetPosition(position, Data.Pivot.TopLeft, BoundsType.AlignAxis);
            }
        }
        
        public float Right
        {
            get => GetPosition(Data.Pivot.TopRight, BoundsType.AlignAxis).x;
            set
            {
                var position = GetPosition(Data.Pivot.TopRight, BoundsType.AlignAxis);
                position.x = value;
                SetPosition(position, Data.Pivot.TopRight, BoundsType.AlignAxis);
            }
        }
        
        public float Top
        {
            get => GetPosition(Data.Pivot.TopLeft, BoundsType.AlignAxis).y;
            set
            {
                var position = GetPosition(Data.Pivot.TopLeft, BoundsType.AlignAxis);
                position.y = value;
                SetPosition(position, Data.Pivot.TopLeft, BoundsType.AlignAxis);
            }
        }
        
        public float Bottom
        {
            get => GetPosition(Data.Pivot.BottomLeft, BoundsType.AlignAxis).y;
            set
            {
                var position = GetPosition(Data.Pivot.BottomLeft, BoundsType.AlignAxis);
                position.y = value;
                SetPosition(position, Data.Pivot.BottomLeft, BoundsType.AlignAxis);
            }
        }

    }
}
