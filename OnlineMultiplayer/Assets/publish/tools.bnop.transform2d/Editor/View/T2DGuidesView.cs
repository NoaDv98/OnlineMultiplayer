//----------------------------------------------
// Transform2D
// Copyright © 2024 BaconOppenheim™
//----------------------------------------------

using System;
using tools.bnop.Transform2d.Data;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace tools.bnop.Transform2d.View
{
    internal struct MouseEvent
    {
        public bool MouseDown;
        public bool MouseUp;
        public bool RightClick;
        public bool DoubleClick;
        public bool Drag;
        public bool StartDrag;
        public T2DGuide HoveredGuide;
        public T2DGuide DraggedGuide;
        public T2DGuide DroppedGuide;
        public T2DGuide DoubleClickedGuide;
        public T2DGuide RightClickedGuide;
        public T2DRuler HoveredRuler;
        public T2DRuler ClickedRuler;
        public T2DRuler RightClickedRuler;
        public T2DRuler DoubleClickedRuler;
        public bool DoubleClickedCorner;
        public bool RightClickCorner;
        public Vector2 MousePosition;
        public bool IsOverRuler;
        public bool IsOverCorner;
        public bool IsMouseDown;
        public bool HasDragStarted;

        public float MouseCoord(Axis axis) => AxisUtils.CoordFromVec2(MousePosition, axis);
        public float MouseCoordOpposite(Axis axis) => AxisUtils.CoordFromVec2(MousePosition, AxisUtils.Opposite(axis));
    }

    public class T2DGuidesView
    {
        internal Action<T2DGuidesViewEvent> Change;

        private T2DGuidesViewData _data;
        private MouseEvent _event;

        private readonly T2DRulersView _rulersView = new();

        private static readonly Color GuideColor = new(0, 1, 1, 0.5f);
        private static readonly Color GuideHoverColor = new(0, 1, 1, 0.8f);

        private static readonly GUIStyle LabelStyle = new()
        {
            fontSize = 11,
            normal = { textColor = Color.white },
            alignment = TextAnchor.MiddleCenter,
            padding = new RectOffset(6, 6, 4, 4),
        };

        public T2DGuidesViewData Data
        {
            set => _data = value;
        }

        private void DispatchChange(T2DGuidesViewEventType type, T2DGuide guide = null, dynamic data = null)
        {
            var e = new T2DGuidesViewEvent()
            {
                Type = type,
                Guide = guide,
                Data = data
            };
            Change?.Invoke(e);
        }

        public void Render()
        {
            var isIsolatedPrefab = PrefabStageUtility.GetCurrentPrefabStage() != null;
            if (isIsolatedPrefab) return;

            Handles.BeginGUI();

            HandleMouseEvent();

            foreach (var guide in (_data.Guides))
            {
                DrawGuide(guide);
            }

            ShowGuideTooltip(_event.DraggedGuide);
            UpdateCursor();
            DrawRulers();

            Handles.EndGUI();

            SceneView.lastActiveSceneView?.Repaint();
        }

        private void HandleMouseEvent()
        {
            _event = GetMouseEvent();

            switch (_event)
            {
                case { ClickedRuler: not null }:
                    var newGuide = new T2DGuide(
                        _event.ClickedRuler.Axis,
                        T2DSceneViewScreen.ToWorldCoord(
                            _event.ClickedRuler.Axis,
                            _event.MouseCoordOpposite(_event.ClickedRuler.Axis),
                            reverseAxis: true
                        )
                    );
                    _event.DraggedGuide = newGuide;
                    DispatchChange(T2DGuidesViewEventType.CreateGuideByDrag, newGuide);
                    TrapMouseEvents = true;
                    break;

                case { DroppedGuide: not null }:
                    if (_event.HoveredRuler?.Axis == _event.DroppedGuide.Axis)
                        DispatchChange(T2DGuidesViewEventType.RemoveGuide, _event.DroppedGuide);
                    else
                        DispatchChange(T2DGuidesViewEventType.DropGuide, _event.DroppedGuide);

                    TrapMouseEvents = false;
                    StopEventPropagation();
                    break;

                case { Drag: true, DraggedGuide: not null }:
                    var position = T2DSceneViewScreen.ToWorldCoord(
                        _event.DraggedGuide.Axis,
                        _event.MouseCoordOpposite(_event.DraggedGuide.Axis),
                        reverseAxis: true
                    );

                    DispatchChange(
                        _event is { StartDrag: true, IsOverRuler: false }
                            ? T2DGuidesViewEventType.StartDragGuide
                            : T2DGuidesViewEventType.DragGuide, _event.DraggedGuide, position);

                    break;

                case { RightClickedGuide: not null }:
                    ShowGuideContextMenu(_event.RightClickedGuide);
                    break;

                case { DoubleClickedGuide: not null }:
                    PromptEditGuide(_event.DoubleClickedGuide);
                    TrapMouseEvents = false;
                    StopEventPropagation();
                    break;

                case { RightClickedRuler: not null }:
                    ShowRulerContextMenu(_event.RightClickedRuler.Axis);
                    break;

                case { RightClickCorner: true }:
                    ShowCornerContextMenu();
                    break;

                case { DoubleClickedRuler: not null }:
                    PromptAddGuide(_event.DoubleClickedRuler!.Axis);
                    TrapMouseEvents = false;
                    StopEventPropagation();
                    break;

                case { DoubleClickedCorner: true }:
                    PromptAddGuide(Axis.Vertical);
                    TrapMouseEvents = false;
                    StopEventPropagation();
                    break;

                case { MouseDown: true, DraggedGuide: not null }:
                    TrapMouseEvents = true;
                    break;

                case { MouseUp: true }:
                    TrapMouseEvents = false;
                    break;
            }
        }

        private MouseEvent GetMouseEvent()
        {
            var e = new MouseEvent()
            {
                HasDragStarted = _event.HasDragStarted,
                DraggedGuide = _event.DraggedGuide,
                IsMouseDown = _event.IsMouseDown,

                
                MouseDown = Event.current.type == EventType.MouseDown && Event.current.button == 0,
                MouseUp = Event.current.type == EventType.MouseUp && Event.current.button == 0,
                RightClick = Event.current.type == EventType.MouseDown && Event.current.button == 1,
                DoubleClick = Event.current.type == EventType.MouseDown && Event.current.clickCount == 2,
                Drag = Event.current.type == EventType.MouseDrag && Event.current.button == 0,
                HoveredRuler = _rulersView.RulerUnderMouse,
                IsOverRuler = _rulersView.RulerUnderMouse != null,
                IsOverCorner = T2DRulersView.IsMouseOverCorner(),
                MousePosition = Event.current.mousePosition
            };

            if (e.MouseDown) e.IsMouseDown = true;
            else if (e.MouseUp || Event.current.type == EventType.Used) e.IsMouseDown = false;

            var guideUnderMouse = _data.Guides.Find(IsMouseOverGuide);

            if (e is { IsMouseDown: false, IsOverRuler: false })
                e.HoveredGuide = guideUnderMouse;

            switch (e)
            {
                case { DoubleClick: true }:
                    e.MouseDown = false;
                    if (e.HoveredRuler != null) e.DoubleClickedRuler = e.HoveredRuler;
                    else if (e.IsOverCorner) e.DoubleClickedCorner = true;
                    else if (guideUnderMouse != null)
                        e.DoubleClickedGuide = guideUnderMouse;
                    break;

                case { MouseDown: true }:
                    if (e.HoveredRuler != null) e.ClickedRuler = e.HoveredRuler;
                    else e.DraggedGuide = guideUnderMouse;
                    break;

                case { MouseUp: true, DraggedGuide: not null }:
                    e.DroppedGuide = e.DraggedGuide;
                    e.DraggedGuide = null;
                    e.HasDragStarted = false;
                    break;

                case { Drag: true }:
                    if (!e.HasDragStarted)
                    {
                        e.StartDrag = true;
                        e.HasDragStarted = true;
                    }
                    break;

                case { RightClick: true }:
                    if (e.HoveredRuler != null) e.RightClickedRuler = e.HoveredRuler;
                    else if (e.IsOverCorner) e.RightClickCorner = true;
                    else e.RightClickedGuide = guideUnderMouse;
                    break;
            }

            return e;
        }


        private void UpdateCursor()
        {
            var isMouseDraggingNonGuide = _event is { IsMouseDown: true, DraggedGuide: null };
            if (_event.DraggedGuide != null || _event.HoveredGuide != null)
                ShowDragCursor(_event.DraggedGuide?.Axis ?? _event.HoveredGuide!.Axis);
            else if (_event.IsOverRuler && !isMouseDraggingNonGuide)
                ShowDragCursor(_event.HoveredRuler.Axis);
        }

        private void DrawGuide(T2DGuide guide)
        {
            var screenCoord = T2DSceneViewScreen.ToScreenCoord(guide);
            var length = guide.IsHorizontal
                ? T2DSceneViewScreen.ScreenRect.width
                : T2DSceneViewScreen.ScreenRect.height;
            var color = guide == _event.DraggedGuide || guide == _event.HoveredGuide ? GuideHoverColor : GuideColor;

            T2DDrawingUtils.DrawLine(guide.Axis, screenCoord, 0, length, color);
        }

        private void DrawRulers()
        {
            _rulersView.UnitType = _data.UnitType;
            _rulersView.SelectionBounds = _data.SelectionBounds;
            _rulersView.Render();
        }

        private bool IsMouseOverGuide(T2DGuide guide)
        {
            var guideScreenPosition = T2DSceneViewScreen.ToScreenCoord(guide);
            var guideLength = guide.IsHorizontal
                ? T2DSceneViewScreen.ScreenRect.width
                : T2DSceneViewScreen.ScreenRect.height;
            var guideRect = guide.IsHorizontal
                ? new Rect(new Vector2(0, guideScreenPosition - 5), new Vector2(guideLength, 10))
                : new Rect(new Vector2(guideScreenPosition - 5, 0), new Vector2(10, guideLength));
            return guideRect.Contains(_event.MousePosition);
        }

        private static void ShowDragCursor(Axis axis)
        {
            var cursor = AxisUtils.IsVertical(axis) ? MouseCursor.ResizeHorizontal : MouseCursor.ResizeVertical;
            EditorGUIUtility.AddCursorRect(SceneView.currentDrawingSceneView.camera.pixelRect, cursor);
        }

        private void ShowGuideTooltip(T2DGuide guide)
        {
            if (guide == null || _event.IsOverRuler) return;
            var isWorldUnits = _data.UnitType == UnitType.WorldUnits;

            var displayPos = isWorldUnits ? guide.Position : T2DScreen.ToPixels(guide.Position);
            var label = $"{displayPos}" + (isWorldUnits ? "" : "px");
            var mouseCoord = _event.MouseCoord(_event.DraggedGuide.Axis);
            var screenPos = T2DSceneViewScreen.ToScreenCoord(guide);
            var size = LabelStyle.CalcSize(new GUIContent(label));
            const int padding = 20;
            var position = new Vector2(
                guide.IsHorizontal ? mouseCoord - size.x / 2 : screenPos + padding,
                guide.IsHorizontal ? screenPos + padding : mouseCoord - size.y / 2
            );

            T2DDrawingUtils.DrawRectangle(position, size, new Color(0.15f, 0.15f, 0.15f));
            GUI.Label(new Rect(position, size), label, LabelStyle);
        }

        private void ShowGuideContextMenu(T2DGuide guide)
        {
            TrapMouseEvents = false;
            StopEventPropagation();
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Edit Guide"), false,
                () => { PromptEditGuide(guide); });
            menu.AddItem(new GUIContent("Delete Guide"), false,
                () => { DispatchChange(T2DGuidesViewEventType.RemoveGuide, guide); });
            menu.ShowAsContext();
        }

        private void ShowRulerContextMenu(Axis axis)
        {
            TrapMouseEvents = false;
            StopEventPropagation();
            var oppositeAxis = AxisUtils.Opposite(axis);
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent($"Clear {oppositeAxis} Guides"), false,
                () => { DispatchChange(T2DGuidesViewEventType.ClearGuides, data: oppositeAxis); });
            menu.AddItem(new GUIContent("Clear All Guides"), false,
                () => { DispatchChange(T2DGuidesViewEventType.ClearGuides); });
            menu.AddItem(new GUIContent($"Add New Guide"), false, () => { PromptAddGuide(axis); });
            menu.ShowAsContext();
        }

        private void ShowCornerContextMenu()
        {
            TrapMouseEvents = false;
            StopEventPropagation();
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Clear All Guides"), false,
                () => { DispatchChange(T2DGuidesViewEventType.ClearGuides); });
            menu.AddItem(new GUIContent($"Add New Guide"), false, () => { PromptAddGuide(Axis.Vertical); });
            menu.ShowAsContext();
        }

        private void PromptAddGuide(Axis axis)
        {
            EditorApplication.delayCall += () =>
            {
                var opAxis = (AxisUtils.Opposite(axis));
                var stub = new T2DGuide(opAxis);
                T2DGuideDialogView.ShowDialog(
                    T2DGuideDialogView.Operation.Create,
                    stub,
                    (result) => { DispatchChange(T2DGuidesViewEventType.CreateGuide, result); }
                );
            };
        }

        private void PromptEditGuide(T2DGuide guide)
        {
            EditorApplication.delayCall += () =>
            {
                T2DGuideDialogView.ShowDialog(
                    T2DGuideDialogView.Operation.Edit,
                    guide,
                    (result) => { DispatchChange(T2DGuidesViewEventType.EditGuide, guide, result); }
                );
            };
        }

        private static bool TrapMouseEvents
        {
            set
            {
                if (!value) GUIUtility.hotControl = GUIUtility.hotControl;
                else
                {
                    var controlId = GUIUtility.GetControlID(FocusType.Passive);
                    GUIUtility.hotControl = controlId;
                    Event.current.Use();
                }
            }
        }

        private static void StopEventPropagation()
        {
            Event.current.Use();
        }
    }
}
