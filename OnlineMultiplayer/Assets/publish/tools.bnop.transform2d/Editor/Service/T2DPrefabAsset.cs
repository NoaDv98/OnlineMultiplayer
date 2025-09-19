//----------------------------------------------
// Transform2D
// Copyright © 2024 BaconOppenheim™
//----------------------------------------------

using UnityEngine;
using UnityEditor;

namespace tools.bnop.Transform2d.Service
{
    public enum PrefabType
    {
        None,
        PrefabAsset,        
        PrefabInstance,     
        RegularObject       
    }
    
    public static class T2DPrefabAsset
    {
        public static PrefabType IsPrefab(Object target)
        {
            var gameObject = (GameObject)target;
            if (gameObject == null)
                return PrefabType.None;

            if (PrefabUtility.IsPartOfPrefabAsset(gameObject))
                return PrefabType.PrefabAsset;

            if (PrefabUtility.IsPartOfPrefabInstance(gameObject))
                return PrefabType.PrefabInstance;

            if (!PrefabUtility.IsPartOfAnyPrefab(gameObject))
                return PrefabType.RegularObject;

            return PrefabType.None;
        }
    }
}
