//----------------------------------------------
// Transform2D
// Copyright © 2024 BaconOppenheim™
//----------------------------------------------

using UnityEditor;
using System;

namespace tools.bnop.Transform2d.Service
{
    public static class T2DUndo
    {
        private static bool _enabled = true;
        private static int _currentGroup;
        private static string _currentGroupName;

        public static void Register(UnityEngine.Object objectToUndo, string name)
        {
            if (_enabled)
                Undo.RegisterCompleteObjectUndo(objectToUndo, $"Transform2D {name}");
        }
        
        public static void Register(UnityEngine.Object[] objectsToUndo, string name)
        {
            if (_enabled)
                Undo.RegisterCompleteObjectUndo(objectsToUndo, $"Transform2D {name}");
        }

        public static void Suppress(Action action)
        {
            _enabled = false;
            action?.Invoke();
            _enabled = true;
        }
        
        public static void BeginGroup(string name)
        {
            if (!_enabled) return;
            _currentGroup = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName($"Transform2D {name}");
        }
        
        public static void EndGroup()
        {
            if (!_enabled || _currentGroup == 0) return;
            Undo.CollapseUndoOperations(_currentGroup);
            _currentGroup = 0;
        }

        public static void RenameCurrentGroup(string removeGuide)
        {
            if (!_enabled || _currentGroup == 0) return;
            Undo.SetCurrentGroupName($"Transform2D {removeGuide}");
        }
    }
}
