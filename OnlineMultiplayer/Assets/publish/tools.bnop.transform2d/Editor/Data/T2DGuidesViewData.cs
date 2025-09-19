//----------------------------------------------
// Transform2D
// Copyright © 2024 BaconOppenheim™
//----------------------------------------------

using System.Collections.Generic;
using UnityEngine;

namespace tools.bnop.Transform2d.Data
{
    public struct T2DGuidesViewData
    {
        public List<T2DGuide> Guides;
        public UnitType UnitType;
        public Bounds? SelectionBounds;
    }
}
