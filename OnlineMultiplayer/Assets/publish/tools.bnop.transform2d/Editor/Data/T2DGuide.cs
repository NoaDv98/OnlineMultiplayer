//----------------------------------------------
// Transform2D
// Copyright © 2024 BaconOppenheim™
//----------------------------------------------

using System;

namespace tools.bnop.Transform2d.Data
{
    [Serializable]
    public class T2DGuide
    {
        public Axis Axis;
        public float Position;

        public bool IsHorizontal => Axis == Axis.Horizontal;
        public bool IsVertical => Axis == Axis.Vertical;
    
        public T2DGuide(Axis axis, float position = 0)
        {
            Axis = axis;
            Position = position;
        }
        
        public T2DGuide(T2DGuide guide)
        {
            Axis = guide.Axis;
            Position = guide.Position;
        }
    }
    
}
