//----------------------------------------------
// Transform2D
// Copyright © 2024 BaconOppenheim™
//----------------------------------------------

using UnityEngine;

namespace tools.bnop.Transform2d.Data
{
    public class T2DNoBounds : T2DObjectBounds<object>
    {
        public T2DNoBounds(GameObject gameObject) : base(gameObject) { }
    }
    
}
