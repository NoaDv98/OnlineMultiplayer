//----------------------------------------------
// Transform2D
// Copyright © 2024 BaconOppenheim™
//----------------------------------------------

using System.Collections.Generic;
using UnityEngine;

namespace tools.bnop.Transform2d.Data
{

    [CreateAssetMenu(menuName = "Transform2D/GuideList")]
    public class T2DGuideData : ScriptableObject
    {
        [SerializeField]
        public List<T2DGuide> list;
    }
}
