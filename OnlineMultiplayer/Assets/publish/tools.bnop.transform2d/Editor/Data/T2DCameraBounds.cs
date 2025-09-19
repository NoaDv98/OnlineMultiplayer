//----------------------------------------------
// Transform2D
// Copyright © 2024 BaconOppenheim™
//----------------------------------------------

using UnityEngine;

namespace tools.bnop.Transform2d.Data
{
    public class T2DCameraBounds : T2DObjectBounds<Camera>
    {

        public T2DCameraBounds(GameObject gameObject) : base(gameObject) {}
        
        public override Bounds ObjectAligned => Component ? GetObjectAlignedBounds() : new Bounds();
        public override Bounds AxisAligned => Component ? GetAxisAlignedBounds() : new Bounds();
        
        public float OrthographicSize => Component ? Component.orthographicSize : 0;
        
        protected override bool ValidateComponent(Camera camera)
        {
            return camera && camera.orthographic;
        }
        
        private Bounds GetObjectAlignedBounds()
        {
            if (!Component) return new Bounds();

            var cameraHeight = Component.orthographicSize * 2;
            var cameraWidth = cameraHeight * Component.aspect;

            var localSize = new Vector3(cameraWidth, cameraHeight, 0);
            var localCenter = Vector3.zero;

            var globalScale = Transform.lossyScale;
            var inverseScale = new Vector3(globalScale.x != 0 ? 1 / globalScale.x : 1,
                globalScale.y != 0 ? 1 / globalScale.y : 1,
                globalScale.z != 0 ? 1 / globalScale.z : 1);
            localSize = Vector3.Scale(localSize , inverseScale);

            var bounds = new Bounds(localCenter, localSize);
            
            return bounds;
        }
        
        private Bounds GetAxisAlignedBounds()
        {
            if (!Component) return new Bounds();

            var cameraHeight = Component.orthographicSize * 2;
            var cameraWidth = cameraHeight * Component.aspect;

            var corners = new Vector3[4];
            corners[0] = new Vector3(-cameraWidth / 2, -cameraHeight / 2, 0); 
            corners[1] = new Vector3(cameraWidth / 2, -cameraHeight / 2, 0);  
            corners[2] = new Vector3(-cameraWidth / 2, cameraHeight / 2, 0);  
            corners[3] = new Vector3(cameraWidth / 2, cameraHeight / 2, 0);   

            var globalScale = Transform.lossyScale;
            var inverseScale = new Vector3(1/globalScale.x, 1/globalScale.y, 1/globalScale.z);
            
            for (var i = 0; i < corners.Length; i++)
            {
                corners[i] = Vector3.Scale(corners[i] , inverseScale);
                corners[i] = Transform.TransformPoint(corners[i]);
            }

            var boundsMin = corners[0];
            var boundsMax = corners[0];
            foreach (var corner in corners)
            {
                boundsMin = Vector3.Min(boundsMin, corner);
                boundsMax = Vector3.Max(boundsMax, corner);
            }

            var bounds = new Bounds();
            bounds.SetMinMax(boundsMin, boundsMax);
            
            return bounds;
        }     
        
    }
}
