//----------------------------------------------
// Transform2D
// Copyright © 2024 BaconOppenheim™
//----------------------------------------------

using UnityEngine;

namespace tools.bnop.Transform2d.Data
{
    public struct T2DViewData
    {
        public bool Is2DMode;
        public int ObjectCount;
        public bool HasPrefab;
        public bool HasNesting;
        public string ActiveGameObjectName;
        public T2DTransformData Transform;
        public T2DSettingsData Settings;
        public BoundsSource ActiveBoundsSource;
        public Vector2 UnscaledSize;
        public Vector2 SpritePivot;
        public bool SpriteIsDownscaled;
        public float SpritePPU;
        public float SpritePPUSetting;
        public bool HasRenderer;
        public bool HasCollider;
        public bool HasText;
        public bool IsMatchingPPU;
        public bool PivotIsPixelPerfect;
        public int CameraCount;
        public int TextCount;
        
        public bool IsNoSelection => ObjectCount == 0;
        public bool IsSingleSelection => ObjectCount == 1;
        public bool IsMultiSelection => ObjectCount > 1;
        public bool HasCamera => CameraCount > 0;
        public bool SizeIsEditable =>
            IsSingleSelection && ActiveBoundsSource is BoundsSource.Collider or BoundsSource.Sprite or BoundsSource.Text;
        
        public bool ScaleIsEditable =>
            IsSingleSelection;
        
        public bool RotationIsEditable =>
            IsSingleSelection;
    }
}
