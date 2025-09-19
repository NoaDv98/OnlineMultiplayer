//----------------------------------------------
// Transform2D
// Copyright © 2024 BaconOppenheim™
//----------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using tools.bnop.Transform2d.Service;
using UnityEngine;

namespace tools.bnop.Transform2d.Data
{
    public class T2DSelection
    {
        internal Action Change;
        public List<T2DGameObject> Children { get; set; } = new();
        public int Count => Children.Count;
        public bool IsEmpty => Count == 0;
        public bool IsSingle => Count == 1;
        public T2DGameObject First => IsEmpty ? null : Children.First();
        public Transform[] Transforms => Children.Select(child => child.Target.transform).ToArray();
        
        internal bool HasPrefab { get; private set; }
        internal bool HasNesting { get; private set; }

        private Vector2 _positionPivot = Data.Pivot.Center;

        public T2DSelection(GameObject[] gameObjects = null)
        {
            if (gameObjects != null) Set(gameObjects);
        }

        public Vector2 Pivot
        {
            get => IsSingle ? First.Pivot : _positionPivot;
            set
            {
                if (IsSingle)
                {
                    First.Pivot = value;
                }

                _positionPivot = value;
            }
        }

        public Vector2 Position
        {
            get
            {
                if (IsEmpty) return Vector2.zero;
                return IsSingle ? First.Position : Center + PositionPivotOffset();
            }
            set
            {
                if (IsSingle)
                {
                    if (value == First.Position) return;
                    T2DUndo.Register(First.Transform, "Transform Position");
                    First.Position = value;
                }
                else
                {
                    var offset = value - PositionPivotOffset() - Center;
                    if (offset == Vector2.zero) return;
                    T2DUndo.Register(Transforms, "Transform Positions");
                    Children.ForEach(child => { child.Position += offset; });
                }
                Change?.Invoke();
            }
        }

        public Vector2 LocalPosition
        {
            get => IsSingle ? First.LocalPosition : Vector2.zero;
            set
            {
                if (IsSingle)
                {
                    if (value == First.Position) return;
                    T2DUndo.Register(First.Transform, "Transform Position");
                    First.LocalPosition = value;
                };
            }
        }

        public void SetPosition(Vector2 position, Vector2 pivot, BoundsType boundsType = BoundsType.AlignAxis)
        {
            if (IsSingle)
            {
                First.SetPosition(position, pivot, boundsType);
                return;
            }
            
            var activePivot = Pivot;
            Pivot = pivot;
            Position = position;
            Pivot = activePivot;
        }

        public Vector2 GetPosition(Vector2 pivot, BoundsType boundsType = BoundsType.AlignAxis)
        {
            if (IsSingle)
                return First.GetPosition(pivot, boundsType);
            
            var activePivot = Pivot;
            Pivot = pivot;
            var position = Position;
            Pivot = activePivot;
            return position;
        }
        
        public void Set(GameObject[] gameObjects)
        {
            HasPrefab = GetHasPrefab(gameObjects);
            HasNesting = GetHasNesting(gameObjects);

            if (!HasPrefab && !HasNesting)
                Children = gameObjects
                    .Select(gameObject => new T2DGameObject(gameObject)).ToList();
            
            else Children = new List<T2DGameObject>();
        }

        public Bounds Bounds => IsSingle ? First.Bounds.AxisAligned : new Bounds(Center, Size);

        public BoundsSource BoundsSource
        {
            set
            {
                Children.ForEach(child => { child.SetBoundsSource(value); });
            }
            get
            {
                return IsEmpty
                    ? BoundsSource.None
                    : IsSingle
                        ? First.BoundsSource
                        : Children
                            .Where(child => !child.Camera)
                            .Min(child => child.BoundsSource);
            }
        }

        public float Left => !IsEmpty ? Children.Min(child => child.Left) : float.NaN;

        public float Right => !IsEmpty ? Children.Max(child => child.Right) : float.NaN;

        public float Top => !IsEmpty ? Children.Max(child => child.Top) : float.NaN;

        public float Bottom => !IsEmpty ? Children.Min(child => child.Bottom) : float.NaN;

        public float HorizontalCenter => (Left + Right) / 2;

        public float VerticalCenter => (Bottom + Top) / 2;

        public Vector2 Center => new(HorizontalCenter, VerticalCenter);


        public Vector2 LocalSize
        {
            get
            {
                if (IsEmpty) return Vector2.zero;
                return IsSingle ? First.LocalSize : new Vector2(Right - Left, Top - Bottom);
            }
            
            set
            {
                if (!IsSingle) throw new Exception("Cannot set size for group");
                if (value == LocalSize) return;
                T2DUndo.Register(First.Target.transform, "Transform Size");
                First.LocalSize = value;
                Change?.Invoke();
            }
        }
        
        public Vector2 Size
        {
            get
            {
                if (IsEmpty) return Vector2.zero;
                return IsSingle ? First.Size : new Vector2(Right - Left, Top - Bottom);
            }
            
            set
            {
                if (!IsSingle) throw new Exception("Cannot set size for group");
                if (value == Size) return;
                T2DUndo.Register(First.Target.transform, "Transform Size");
                First.Size = value;
                Change?.Invoke();
            }
        }
        
        public Vector2 LocalScale
        {
            get => IsSingle ? First.LocalScale : Vector2.one;

            set
            {
                if (!IsSingle) throw new Exception("Cannot set scale for group");
                if (value == LocalScale) return;
                T2DUndo.Register(First.Target.transform, "Transform Scale");
                First.LocalScale = value;
                Change?.Invoke();
            }
        }
        
        public Vector2 Scale
        {
            get => IsSingle ? First.Scale : Vector2.one;

            set
            {
                if (!IsSingle) throw new Exception("Cannot set scale for group");
                if (value == Scale) return;
                T2DUndo.Register(First.Target.transform, "Transform Scale");
                First.Scale = value;
                Change?.Invoke();
            }
        }
        
        public float Rotation
        {
            get => IsSingle ? First.Rotation : 0f;

            set
            {
                if (!IsSingle) throw new Exception("Cannot set rotation for group");
                if (Mathf.Approximately(value, Rotation)) return;
                T2DUndo.Register(First.Target.transform, "Transform Rotation");
                First.Rotation = value;
                Change?.Invoke();
            }
        }
        
        public float LocalRotation
        {
            get => IsSingle ? First.LocalRotation : 0f;

            set
            {
                if (!IsSingle) throw new Exception("Cannot set local rotation for group");
                if (Mathf.Approximately(value, LocalRotation)) return;
                T2DUndo.Register(First.Target.transform, "Transform Local Rotation");
                First.LocalRotation = value;
                Change?.Invoke();
            }
        }
        
        public Vector2 UnscaledSize => IsSingle ? First.Bounds.ObjectAligned.size : LocalSize;

        public void Align(AlignOptions anchor)
        {
            T2DUndo.Register(Transforms, "Align");
            float coord;
            switch (anchor)
            {
                case AlignOptions.Left:
                    coord = Left;
                    Children.ForEach(child => { child.Left = coord; });
                    break;

                case AlignOptions.Right:
                    coord = Right;
                    Children.ForEach(child => { child.Right = coord; });
                    break;

                case AlignOptions.Top:
                    coord = Top;
                    Children.ForEach(child => { child.Top = coord; });
                    break;

                case AlignOptions.Bottom:
                    coord = Bottom;
                    Children.ForEach(child => { child.Bottom = coord; });
                    break;

                case AlignOptions.HorizontalCenter:
                    coord = HorizontalCenter;
                    Children.ForEach(child => { child.HorizontalCenter = coord; });
                    break;

                case AlignOptions.VerticalCenter:
                    coord = VerticalCenter;
                    Children.ForEach(child => { child.VerticalCenter = coord; });
                    break;
            }
            Change?.Invoke();
        }

        public void Distribute(DistributeData data)
        {
            if (Children.Count() < 3 && float.IsNaN(data.Space)) return;
            T2DUndo.Register(Transforms, "Distribute");

            T2DGameObject[] sortedGameObjects;
            float min;
            float max;
            float netSize;

            switch (data.Option)
            {
                case DistributeOptions.Left:
                    sortedGameObjects = Sort(Children, DistributeOptions.VerticalSpacing);
                    sortedGameObjects = Sort(sortedGameObjects, data.Option);
                    min = sortedGameObjects.First().Left;
                    max = sortedGameObjects.Last().Left;
                    data.Space = float.IsNaN(data.Space) ? (max - min) / (sortedGameObjects.Count() - 1) : data.Space;
                    for (var i = 1; i < sortedGameObjects.Count(); i++)
                    {
                        var child = sortedGameObjects[i];
                        child.Left = min + data.Space * i;
                    }

                    break;

                case DistributeOptions.HorizontalCenters:
                    sortedGameObjects = Sort(Children, DistributeOptions.VerticalSpacing);
                    sortedGameObjects = Sort(sortedGameObjects, data.Option);
                    min = sortedGameObjects.First().HorizontalCenter;
                    max = sortedGameObjects.Last().HorizontalCenter;
                    data.Space = float.IsNaN(data.Space) ? (max - min) / (sortedGameObjects.Count() - 1) : data.Space;
                    for (var i = 1; i < sortedGameObjects.Count(); i++)
                    {
                        var child = sortedGameObjects[i];
                        child.HorizontalCenter = min + data.Space * i;
                    }

                    break;

                case DistributeOptions.Right:
                    sortedGameObjects = Sort(Children, DistributeOptions.VerticalSpacing);
                    sortedGameObjects = Sort(sortedGameObjects, data.Option);
                    min = sortedGameObjects.First().Right;
                    max = sortedGameObjects.Last().Right;
                    data.Space = float.IsNaN(data.Space) ? (max - min) / (sortedGameObjects.Count() - 1) : data.Space;
                    for (var i = 1; i < sortedGameObjects.Count(); i++)
                    {
                        var child = sortedGameObjects[i];
                        child.Right = min + data.Space * i;
                    }

                    break;

                case DistributeOptions.Top:
                    sortedGameObjects = Sort(Children, DistributeOptions.HorizontalSpacing);
                    sortedGameObjects = Sort(sortedGameObjects, data.Option);
                    min = sortedGameObjects.First().Top;
                    max = sortedGameObjects.Last().Top;
                    data.Space = float.IsNaN(data.Space) ? (max - min) / (sortedGameObjects.Count() - 1) : data.Space;
                    for (var i = 1; i < sortedGameObjects.Count(); i++)
                    {
                        var child = sortedGameObjects[i];
                        child.Top = min + data.Space * i;
                    }

                    break;

                case DistributeOptions.VerticalCenters:
                    sortedGameObjects = Sort(Children, DistributeOptions.HorizontalSpacing);
                    sortedGameObjects = Sort(sortedGameObjects, data.Option);
                    min = sortedGameObjects.First().VerticalCenter;
                    max = sortedGameObjects.Last().VerticalCenter;
                    data.Space = float.IsNaN(data.Space) ? (max - min) / (sortedGameObjects.Count() - 1) : data.Space;
                    for (var i = 1; i < sortedGameObjects.Count(); i++)
                    {
                        var child = sortedGameObjects[i];
                        child.VerticalCenter = min + data.Space * i;
                    }

                    break;

                case DistributeOptions.Bottom:
                    sortedGameObjects = Sort(Children, DistributeOptions.HorizontalSpacing);
                    sortedGameObjects = Sort(sortedGameObjects, data.Option);
                    min = sortedGameObjects.First().Bottom;
                    max = sortedGameObjects.Last().Bottom;
                    data.Space = float.IsNaN(data.Space) ? (max - min) / (sortedGameObjects.Count() - 1) : data.Space;
                    for (var i = 1; i < sortedGameObjects.Count(); i++)
                    {
                        var child = sortedGameObjects[i];
                        child.Bottom = min + data.Space * i;
                    }

                    break;

                case DistributeOptions.HorizontalSpacing:
                    min = Left;
                    max = Right;
                    sortedGameObjects = Sort(Children, DistributeOptions.VerticalSpacing);
                    sortedGameObjects = Sort(sortedGameObjects, data.Option);
                    netSize = Children.Sum(child => child.Bounds.AxisAligned.size.x);
                    data.Space = float.IsNaN(data.Space) ? (max - min - netSize) / (sortedGameObjects.Count() - 1) : data.Space;
                    for (var i = 1; i < sortedGameObjects.Count(); i++)
                    {
                        var child = sortedGameObjects[i];
                        var prevChild = sortedGameObjects[i - 1];
                        child.Left = prevChild.Right + data.Space;
                    }

                    break;

                case DistributeOptions.VerticalSpacing:
                    min = Bottom;
                    max = Top;
                    sortedGameObjects = Sort(Children, DistributeOptions.HorizontalSpacing);
                    sortedGameObjects = Sort(sortedGameObjects, data.Option);
                    netSize = Children.Sum(child => child.Bounds.AxisAligned.size.y);
                    data.Space = float.IsNaN(data.Space) ? (max - min - netSize) / (sortedGameObjects.Count() - 1) : data.Space;
                    for (var i = 1; i < sortedGameObjects.Count(); i++)
                    {
                        var child = sortedGameObjects[i];
                        var prevChild = sortedGameObjects[i - 1];
                        child.Bottom = prevChild.Top + data.Space;
                    }

                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(data.Option), data.Option, null);
                
            }
            Change?.Invoke();
        }

        private T2DGameObject[] Sort(IEnumerable<T2DGameObject> children, DistributeOptions axis)
        {
            return axis switch
            {
                DistributeOptions.Left => children.OrderBy(child => child.Left)
                    .ToArray(),

                DistributeOptions.HorizontalCenters => children.OrderBy(child => child.HorizontalCenter)
                    .ToArray(),

                DistributeOptions.Right => children.OrderBy(child => child.Right)
                    .ToArray(),

                DistributeOptions.HorizontalSpacing =>
                    children
                        .OrderBy(child =>
                        {
                            var min = Left;
                            var max = Right;
                            if (Mathf.Approximately(child.Left, min)) return min;
                            if (Mathf.Approximately(child.Right, max)) return max;
                            return child.HorizontalCenter;
                        }).ToArray(),

                DistributeOptions.Top => children.OrderBy(child => child.Top)
                    .ToArray(),

                DistributeOptions.VerticalCenters => children.OrderBy(child => child.VerticalCenter)
                    .ToArray(),

                DistributeOptions.Bottom => children.OrderBy(child => child.Bottom)
                    .ToArray(),

                DistributeOptions.VerticalSpacing =>
                    children
                        .OrderBy(child =>
                        {
                            var min = Bottom;
                            var max = Top;
                            if (Mathf.Approximately(child.Bottom, min)) return min;
                            if (Mathf.Approximately(child.Top, max)) return max;
                            return child.VerticalCenter;
                        }).ToArray(),
                _ => throw new ArgumentOutOfRangeException(nameof(axis), axis, null)
            };
        }

        private Vector2 PositionPivotOffset()
        {
            var pivotOffset = (Pivot * LocalSize) - LocalSize * 0.5f;
            return pivotOffset;
        }

        private static bool GetHasPrefab(GameObject[] gameObjects)
        {
            return gameObjects.Any(gameObject => T2DPrefabAsset.IsPrefab(gameObject) == Service.PrefabType.PrefabAsset);
        }

        private static bool GetHasNesting(GameObject[] gameObjects)
        {
            for (var i = 0; i < gameObjects.Count(); i++)
            {
                for (var j = 0; j < gameObjects.Count(); j++)
                {
                    if (i != j && IsAncestor(gameObjects[i], gameObjects[j]))
                        return true;
                }
            }
            return false;
        }
        
        private static bool IsAncestor(GameObject potentialAncestor, GameObject descendant)
        {
            var current = descendant.transform.parent;
            while (current)
            {
                if (current == potentialAncestor.transform)
                    return true;
                current = current.parent;
            }

            return false;
        }
    }
}
