//----------------------------------------------
// Transform2D
// Copyright © 2024 BaconOppenheim™
//----------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using tools.bnop.Transform2d.Service;
using UnityEditor;
using UnityEngine;

namespace tools.bnop.Transform2d.Data
{
    public static class T2DGuides
    {
        private const string RelativePath = "Editor/Store/Guides.asset";
        private static GUID _assetId;

        private static T2DGuideData _data;
        private static bool AssetExists => _data != null;

        private static bool _isCreatingGuide;

        public static void Init()
        {
            if (AssetExists) return;
            _assetId = GetOrCreateAsset();
        }

        public static List<T2DGuide> ListAll
        {
            get
            {
                if (AssetExists) return _data.list;
                Init();
                return _data.list;
            }
        }

        public static List<T2DGuide> ListVertical => ListAll.Where(guide => guide.Axis == Axis.Vertical).ToList();

        public static List<T2DGuide> ListHorizontal => ListAll.Where(guide => guide.Axis == Axis.Horizontal).ToList();

        public static List<T2DGuide> ListHorizontalVisible
        {
            get
            {
                var bounds = T2DSceneViewScreen.Bounds;
                return ListHorizontal.Where(guide => guide.Position > bounds.min.y && guide.Position < bounds.max.y)
                    .ToList();
            }
        }

        public static List<T2DGuide> ListVerticalVisible
        {
            get
            {
                var bounds = T2DSceneViewScreen.Bounds;
                return ListVertical.Where(guide => guide.Position > bounds.min.x && guide.Position < bounds.max.x)
                    .ToList();
            }
        }

        private static GUID GetOrCreateAsset()
        {
            _data = T2DSerializedObject.FindOrCreateInRelativePath<T2DGuideData>(RelativePath);
            _data.list ??= new List<T2DGuide>(); 
            return AssetDatabase.GUIDFromAssetPath(T2DAssets.ResolveRelativeAssetPath(RelativePath));
        }

        public static void OnGuidesViewChange(T2DGuidesViewEvent guidesViewChange)
        {
            switch (guidesViewChange.Type)
            {
                case T2DGuidesViewEventType.CreateGuide:
                    AddGuide(guidesViewChange.Guide);
                    break;
                case T2DGuidesViewEventType.CreateGuideByDrag:
                    AddDraggedGuide(guidesViewChange.Guide);
                    break;
                case T2DGuidesViewEventType.EditGuide:
                    EditGuide(guidesViewChange.Guide, guidesViewChange.Data);
                    break;
                case T2DGuidesViewEventType.RemoveGuide:
                    RemoveGuide(guidesViewChange.Guide);
                    break;
                case T2DGuidesViewEventType.StartDragGuide:
                    OnGuideStartDrag(guidesViewChange.Guide, guidesViewChange.Data);
                    break;
                case T2DGuidesViewEventType.DragGuide:
                    MoveGuide(guidesViewChange.Guide, guidesViewChange.Data);
                    break;
                case T2DGuidesViewEventType.DropGuide:
                    OnGuideDrop();
                    break;
                case T2DGuidesViewEventType.ClearGuides:
                    ClearGuides(guidesViewChange.Data);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void AddGuide(T2DGuide guide)
        {
            T2DUndo.Register(_data, "Create Guide");
            ListAll.Add(guide);
        }

        private static void AddDraggedGuide(T2DGuide guide)
        {
            T2DUndo.BeginGroup("Create Guide");
            _isCreatingGuide = true;
            AddGuide(guide);
        }

        private static void RemoveGuide(T2DGuide guide)
        {
            T2DUndo.Register(_data, "Remove Guide");
            ListAll.Remove(guide);
            Save();
            T2DUndo.RenameCurrentGroup("Remove Guide");
            T2DUndo.EndGroup();
        }

        private static void EditGuide(T2DGuide guide, T2DGuide data)
        {
            T2DUndo.Register(_data, "Edit Guide");
            guide.Position = data.Position;
            guide.Axis = data.Axis;
            Save();
        }

        private static void MoveGuide(T2DGuide guide, float position)
        {
            T2DUndo.Register(_data, _isCreatingGuide ? "Create Guide" : "Move Guide");
            if (!Transform2D.Model.Settings.SnapToPixel)
            {
                guide.Position = position;
                return;
            }

            var position2D = guide.Axis == Axis.Horizontal ? new Vector2(0, position) : new Vector2(position, 0);
            var snapped = T2DScreen.SnapToPixels(position2D);
            guide.Position = guide.Axis == Axis.Horizontal ? snapped.y : snapped.x;
        }

        private static void ClearGuides(Axis? axis)
        {
            T2DUndo.Register(_data, "Clear Guides");
            if (!axis.HasValue)
                ListAll.Clear();
            else
                ListAll.RemoveAll(guide => guide.Axis == axis);

            Save();
        }

        private static void OnGuideStartDrag(T2DGuide guide, float position)
        {
            T2DUndo.BeginGroup("Move Guide");
            MoveGuide(guide, position);
        }

        private static void OnGuideDrop()
        {
            _isCreatingGuide = false;
            Save();
            T2DUndo.EndGroup();
        }

        private static void Save()
        {
            if (!AssetExists)
            {
                Init();
                return;
            }

            EditorUtility.SetDirty(_data);
            AssetDatabase.SaveAssetIfDirty(_assetId);
        }

        public static bool IsSnap(Bounds bounds, float maxDistance, Axis axis = Axis.Horizontal)
        {
            var guideList = axis == Axis.Horizontal ? ListHorizontalVisible : ListVerticalVisible;
            IEnumerable<T2DGuide> guides;

            if (axis == Axis.Vertical)
                guides = guideList.Where(guide =>
                    (Mathf.Abs(guide.Position - bounds.min.x) < maxDistance ||
                     Mathf.Abs(guide.Position - bounds.max.x) < maxDistance) ||
                    Mathf.Abs(guide.Position - bounds.center.x) < maxDistance);
            else
                guides = guideList.Where(guide =>
                    (Mathf.Abs(guide.Position - bounds.min.y) < maxDistance ||
                     Mathf.Abs(guide.Position - bounds.max.y) < maxDistance) ||
                    Mathf.Abs(guide.Position - bounds.center.y) < maxDistance);

            return guides.Any();
        }

        public static T2DGuide GetGuideFor(float position, float maxDistance, Axis axis = Axis.Horizontal)
        {
            var guideList = axis == Axis.Horizontal ? ListHorizontalVisible : ListVerticalVisible;

            return guideList.Where(guide => Mathf.Abs(guide.Position - position) < maxDistance)
                .OrderBy(guide => Mathf.Abs(guide.Position - position))
                .FirstOrDefault();
        }
    }
}
