//----------------------------------------------
// Transform2D
// Copyright © 2024 BaconOppenheim™
//----------------------------------------------

using UnityEngine;

namespace tools.bnop.Transform2d.Data
{
    public struct T2DEditorEvent
    {
        public GameObject[] Selection;
        public T2DEditorEventType Type;
        public string Origin;
        public dynamic Data;
    }

    public enum T2DEditorEventType
    {
        Selection,
        GameObject,
        Transform,
        TrackedComponent,
        TrackedProperty,
        SceneViewMode
    }

    public enum T2DEditorTransformEventType
    {
        Move,
        Resize,
        Rotate,
        None
    }

    public struct T2DViewChangeEvent
    {
        public T2DViewChangeEventType Type;
        public dynamic Data;
    }

    public enum T2DViewChangeEventType
    {
        Transform,
        Position,
        Size,
        Scale,
        Rotation,
        Settings
    }

    public struct T2DViewClickEvent
    {
        public T2DViewClickEventType Type;
        public dynamic Data;
    }

    public enum T2DViewClickEventType
    {
        Align,
        Distribute,
        FixSpritePpu,
        FixSpritePivot,
        OpenSpriteEditor,
        FindSpriteAsset
    }

    public struct T2DGuidesViewEvent
    {
        public T2DGuidesViewEventType Type;
        public T2DGuide Guide;
        public dynamic Data;
    }

    public enum T2DGuidesViewEventType
    {
        CreateGuide,
        CreateGuideByDrag,
        RemoveGuide,
        EditGuide,
        StartDragGuide,
        DragGuide,
        DropGuide,
        ClearGuides,
    }
}
