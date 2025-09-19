//----------------------------------------------
// Transform2D
// Copyright © 2024 BaconOppenheim™
//----------------------------------------------

using System;
using System.Linq;
using tools.bnop.Transform2d.Service;
using UnityEngine;

namespace tools.bnop.Transform2d.Data
{
    public class T2DModel
    {
        internal Action Change;
        public T2DSelection Selection { get; private set; }

        private T2DEditorApplication _editorApplication;
        private T2DSettingsData _settings;

        internal void Init()
        {
            _settings = T2DSettings.Load();
            
            if (IsEnabled)
                StartEditorApplication();
            
            else DispatchChange();
        }

        internal void Destroy()
        {
            StopEditorApplication();
        }

        private void StartEditorApplication()
        {
            if (_editorApplication != null) return;
            _editorApplication = new T2DEditorApplication();
            _editorApplication.Change += OnEditorApplicationChange;
            _editorApplication.Init();
        }
        

        private void StopEditorApplication()
        {
            if (_editorApplication == null) return;
            _editorApplication.Change -= OnEditorApplicationChange;
            _editorApplication.Destroy();
            _editorApplication = null;
        }

        private void UpdateSelection(GameObject[] gameObjects = null)
        {
            if (Selection != null)
                Selection.Change -= DispatchChange;
            if (gameObjects != null)
                Selection = new T2DSelection(gameObjects);

            if (Selection == null) return;
            Selection.Pivot = Pivot.ValueOf(_settings.PivotType);
            Selection.BoundsSource = _settings.BoundsSource;
            Selection.Change += DispatchChange;
            DispatchChange();
        }

        private void RebuildSelection()
        {
            UpdateSelection(Selection.Children.Select(child => child.Target).ToArray());
        }

        private bool IsEnabled
        {
            get => _settings.IsEnabled;
            set
            {
                if (_settings.IsEnabled == value) return;
                _settings.IsEnabled = value;
                if (value) StartEditorApplication();
                else
                    StopEditorApplication();
            }
        }

        internal T2DSettingsData Settings
        {
            get => _settings;
            set
            {
                IsEnabled = value.IsEnabled;

                _settings = value;
                T2DSettings.Save(_settings);

                if (_settings.IsEnabled)
                    UpdateSelection();
            }
        }

        public T2DTransformData Transform
        {
            get
            {
                var activeUnitSpace = Selection.IsSingle ? Settings.UnitSpace : UnitSpace.Global;
                var positionUnits = activeUnitSpace == UnitSpace.Global
                    ? Selection.Position
                    : Selection.LocalPosition;
                var sizeUnits = activeUnitSpace == UnitSpace.Global
                    ? Selection.Size
                    : Selection.LocalSize;
                var scale = activeUnitSpace == UnitSpace.Global
                    ? Selection.Scale
                    : Selection.LocalScale;
                var rotation = activeUnitSpace == UnitSpace.Global
                    ? Selection.Rotation
                    : Selection.LocalRotation;

                return new T2DTransformData
                {
                    Position = Settings.UnitType == UnitType.Pixels
                        ? T2DScreen.ToPixels(positionUnits)
                        : positionUnits,
                    Size = Settings.UnitType == UnitType.Pixels
                        ? T2DScreen.ToPixels(sizeUnits)
                        : sizeUnits,
                    Scale = scale,
                    Rotation = rotation,
                };
            }

            set
            {
                var positionUnits = Settings.UnitType == UnitType.Pixels
                    ? T2DScreen.ToUnits(value.Position)
                    : value.Position;

                var activeUnitSpace = Selection.IsSingle ? Settings.UnitSpace : UnitSpace.Global;
                if (activeUnitSpace == UnitSpace.Global)
                    Selection.Position = positionUnits;
                else
                    Selection.LocalPosition = positionUnits;

                if (!Selection.IsSingle) return;
                
                var sizeUnits = Settings.UnitType == UnitType.Pixels
                    ? T2DScreen.ToUnits(value.Size)
                    : value.Size;

                if (activeUnitSpace == UnitSpace.Global)
                    Selection.Size = sizeUnits;
                else
                    Selection.LocalSize = sizeUnits;
                
                var scale = value.Scale;

                if (activeUnitSpace == UnitSpace.Global)
                    Selection.Scale = scale;
                else
                    Selection.LocalScale = scale;
                
                var rotation = value.Rotation;
                if (activeUnitSpace == UnitSpace.Global)
                    Selection.Rotation = rotation;
                else
                    Selection.LocalRotation = rotation;
            }
        }

        public void SetPosition(Vector2 value)
        {
            var positionUnits = Settings.UnitType == UnitType.Pixels
                ? T2DScreen.ToUnits(value)
                : value;

            var activeUnitSpace = Selection.IsSingle ? Settings.UnitSpace : UnitSpace.Global;
            if (activeUnitSpace == UnitSpace.Global)
                Selection.Position = positionUnits;
            else
                Selection.LocalPosition = positionUnits;
        }

        public void SetSize(Vector2 value)
        {
            if (!Selection.IsSingle) return;
            var activeUnitSpace = Settings.UnitSpace;
                
            var sizeUnits = Settings.UnitType == UnitType.Pixels
                ? T2DScreen.ToUnits(value)
                : value;

            if (activeUnitSpace == UnitSpace.Global)
                Selection.Size = sizeUnits;
            else
                Selection.LocalSize = sizeUnits;
        }       
        
        public void SetScale(Vector2 value)
        {
            if (!Selection.IsSingle) return;
            var activeUnitSpace = Settings.UnitSpace;
                
            var scale = value;

            if (activeUnitSpace == UnitSpace.Global)
                Selection.Scale = scale;
            else
                Selection.LocalScale = scale;
        }
        
        public void SetRotation(float value)
        {
            if (!Selection.IsSingle) return;
            var activeUnitSpace = Settings.UnitSpace;

            if (activeUnitSpace == UnitSpace.Global)
                Selection.Rotation = value;
            else
                Selection.LocalRotation = value;
        }
        
        private void DispatchChange()
        {
            if (_settings.IsEnabled)
                _editorApplication.ResetTransformChanges();
            Change?.Invoke();
        }

        private void SnapPosition()
        {
            T2DUndo.Suppress(() =>
            {
                var snap = T2DSnapping.SnapPosition(Selection);
                Selection.SetPosition(snap.Position, snap.Anchor);
            });
        }

        internal void FixSpriteAssetPpu()
        {
            if (Selection.IsEmpty) return;
            T2DDSpriteAsset.SetPixelsPerUnit(Selection.First, Settings.ProjectPpu, RebuildSelection);
        }

        internal void FixSpriteAssetPivot()
        {
            if (Selection.IsEmpty) return;
            T2DDSpriteAsset.SnapPivotToPixel(Selection.First.Target, false);
        }

        internal Vector2 SpritePivot
        {
            get
            {
                if (!Selection.IsSingle && !HasRenderer) return Vector2.zero;
                return Settings.UnitType == UnitType.Pixels
                    ? T2DScreen.ToPixels(Selection.First.SpritePivot)
                    : Selection.First.SpritePivot;
            }
        }

        internal bool Is2DMode => T2DEditorApplication.Is2DMode;

        internal float SpritePpu =>
            Selection.IsSingle && HasRenderer ? Selection.First.SpritePPU : 0;

        internal float SpritePPUSetting =>
            Selection.IsSingle && HasRenderer ? Selection.First.SpritePPUSetting : 0;

        internal bool SpriteIsDownscaled =>
            Selection.IsSingle && HasRenderer && Selection.First.IsSpriteDownscaled;
        
        internal bool HasRenderer =>
            !Selection.IsEmpty && Selection.Children.Any(gameObject => gameObject.Renderer);

        internal bool HasCollider =>
            !Selection.IsEmpty && Selection.Children.Any(gameObject => gameObject.Collider);

        internal bool HasText =>
            !Selection.IsEmpty && Selection.Children.Any(gameObject => gameObject.Text && !gameObject.Camera);

        internal int CameraCount =>
            Selection.Children.Count(gameObject => gameObject.Camera);

        internal int TextCount =>
            Selection.Children.Count(gameObject => gameObject.Text && !gameObject.Camera);

        internal bool IsMatchingPPU => Selection.Children.All(child =>
            !child.Renderer || Mathf.Approximately(child.SpritePPUSetting, Settings.ProjectPpu));

        internal bool IsPivotPixelPerfect =>
            !Selection.IsSingle || !HasRenderer || !Settings.SnapToPixel ||
            T2DDSpriteAsset.IsPivotPixelPerfect(Selection.First.Target, false);

        private void OnEditorApplicationChange(T2DEditorEvent editorEvent)
        {
            T2DConsole.Log($"OnEditorApplicationChange: {editorEvent.Type} : {editorEvent.Data}");

            if (editorEvent.Type == T2DEditorEventType.SceneViewMode)
            {
                DispatchChange();
                return;
            }

            if (editorEvent.Type != T2DEditorEventType.Selection && (Selection.HasNesting || Selection.HasPrefab))
                return;

            switch (editorEvent.Type)
            {
                case T2DEditorEventType.Selection:
                case T2DEditorEventType.GameObject:
                    UpdateSelection(editorEvent.Selection);
                    break;

                case T2DEditorEventType.Transform:
                    if (Settings.Snapping && editorEvent is { Origin: "Scene", Data: T2DEditorTransformEventType.Move })
                        SnapPosition();


                    DispatchChange();

                    break;

                case T2DEditorEventType.TrackedProperty:
                    if (editorEvent.Data == "sprite" || editorEvent.Data == "camera.orthographic")
                        UpdateSelection(editorEvent.Selection);
                    else
                        DispatchChange();
                    break;
                case T2DEditorEventType.TrackedComponent:
                    DispatchChange();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
