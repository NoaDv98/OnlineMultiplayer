//----------------------------------------------
// Transform2D
// Copyright © 2024 BaconOppenheim™
//----------------------------------------------

using TMPro;
using UnityEditor;
using UnityEngine;

namespace tools.bnop.Transform2d.Data
{
    public class T2DTextBounds : T2DObjectBounds<TextMeshPro>
    {

        public T2DTextBounds(GameObject gameObject) : base(gameObject) {}
        
        public override Bounds ObjectAligned => Component ? GetObjectAlignedBounds() : new Bounds();
        public override Bounds AxisAligned => Component ? GetAxisAlignedBounds() : new Bounds();
        
        private Bounds GetObjectAlignedBounds()
        {
            if (!Component) return new Bounds();

            var rect = Component.rectTransform.rect;

            var bounds = new Bounds(
                center: new Vector3(rect.center.x, rect.center.y, 0),
                size: new Vector3(rect.width, rect.height, 0)
            );
            
            return bounds;
        }
        
        private Bounds GetAxisAlignedBounds()
        {
            if (!Component) return new Bounds();

            var corners = new Vector3[4];
            Component.rectTransform.GetWorldCorners(corners);

            float minX = Mathf.Infinity, minY = Mathf.Infinity;
            float maxX = -Mathf.Infinity, maxY = -Mathf.Infinity;

            foreach (Vector3 corner in corners)
            {
                minX = Mathf.Min(minX, corner.x);
                maxX = Mathf.Max(maxX, corner.x);
    
                minY = Mathf.Min(minY, corner.y);
                maxY = Mathf.Max(maxY, corner.y);
            }

            var bounds = new Bounds
            {
                center = new Vector3((minX + maxX) * 0.5f, (minY + maxY) * 0.5f, corners[0].z),
                size = new Vector3(maxX - minX, maxY - minY, 0)
            };

            return bounds;
        }     
    }
}
