//----------------------------------------------
// Transform2D
// Copyright © 2024 BaconOppenheim™
//----------------------------------------------

using UnityEngine;

namespace tools.bnop.Transform2d.Data
{
    public class T2DSpriteBounds : T2DObjectBounds<SpriteRenderer>
    {

        public T2DSpriteBounds(GameObject gameObject):base(gameObject) {}
        
        public override Bounds ObjectAligned => Component ? Component.sprite.bounds : new Bounds();
        public override Bounds AxisAligned => Component ? Component.bounds : new Bounds();
        
        public float SpritePPU => Component ? Component.sprite.pixelsPerUnit : float.NaN;
        public Vector2 SpritePivot => Component ? Component.sprite.pivot / SpritePPU : Vector2.zero;
        
        protected override bool ValidateComponent(SpriteRenderer renderer)
        {
            return renderer && renderer.sprite;
        }
        
    }
}
