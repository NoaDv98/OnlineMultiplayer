//----------------------------------------------
// Transform2D
// Copyright © 2024 BaconOppenheim™
//----------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using tools.bnop.Transform2d.Data;
using UnityEditor;
using UnityEngine;

namespace tools.bnop.Transform2d.Service
{
    public class T2DEditorApplication
    {
        public Action<T2DEditorEvent> Change;

        private GameObject _activeGameObject;
        private SerializedObject _serializedActiveGameObject;
        private SerializedObject _serializedTransform;
        private List<SerializedObject> _trackedComponents = new();
        private readonly List<T2DTrackedProperty> _trackedProperties = new();
        private readonly Dictionary<string, T2DTrackedProperty> _trackedTransformProperties = new();
        private bool _is2DMode;

        private static readonly Type[] TrackComponentTypes =
        {
            typeof(SpriteRenderer),
            typeof(BoxCollider2D),
            typeof(CircleCollider2D),
            typeof(CapsuleCollider2D),
            typeof(PolygonCollider2D),
            typeof(EdgeCollider2D),
            typeof(Camera)
        };

        private static string FocusedWindow =>
            EditorWindow.focusedWindow?.titleContent.ToString();

        private bool GameObjectHasChanged =>
            _serializedActiveGameObject != null &&
            _serializedActiveGameObject.UpdateIfRequiredOrScript();

        private bool TransformHasChanged =>
            _serializedTransform != null &&
            _serializedTransform.UpdateIfRequiredOrScript();

        private bool TrackedComponentHaveChanged =>
            _trackedComponents.Any(serializedComponent =>
                serializedComponent != null &&
                serializedComponent.UpdateIfRequiredOrScript());

        private bool TrackedPropertiesHaveChanged =>
            _trackedProperties.Any(trackedProperty => trackedProperty.HasChanged);

        private string GetFirstModifiedTrackedProperty() =>
            _trackedProperties.FirstOrDefault(trackedProperty => trackedProperty.HasChanged)?.Name;

        private string[] GetModifiedTrackedProperties() =>
            _trackedProperties.Where(trackedProperty => trackedProperty.HasChanged)
                .Select(trackedProperty => trackedProperty.Name).ToArray();

        internal void ResetGameObjectChanges() => _serializedActiveGameObject?.Update();
        internal void ResetTransformChanges() => _serializedTransform?.Update();
        internal void ResetComponentListChanges() => _trackedComponents.ForEach(component => component?.Update());

        internal void ResetTrackedPropertyChanges() =>
            _trackedProperties.ForEach(property => property.HasChanged = false);

        internal void ResetAllChanges()
        {
            ResetTransformChanges();
            ResetGameObjectChanges();
            ResetComponentListChanges();
            ResetTrackedPropertyChanges();
        }

        private void DispatchChange(T2DEditorEventType eventType, dynamic data = null)
        {
            var e = new T2DEditorEvent
            {
                Selection = Selection.gameObjects,
                Origin = FocusedWindow,
                Type = eventType,
                Data = data
            };
            Change?.Invoke(e);
        }

        public void Init()
        {
            Selection.selectionChanged += OnSelectionChange;
            OnSelectionChange();
        }

        public void Destroy()
        {
            Selection.selectionChanged -= OnSelectionChange;
            ClearTrackingCache();
            StopTracking();
        }

        private void OnSelectionChange()
        {
            StartTracking(Selection.activeGameObject);
            DispatchChange(T2DEditorEventType.Selection);
        }

        internal GameObject ActiveGameObject
        {
            get => _activeGameObject;
            set => StartTracking(value);
        }

        private void StartTracking(GameObject gameObject)
        {
            T2DConsole.Log($"EditorApplication.StartTracking: {gameObject?.name}");

            StopTracking();
            ClearTrackingCache();

            if (gameObject)
                BuildTrackingCache(gameObject);
            
            EditorApplication.update += OnEditorApplicationUpdate;
        }

        private void BuildTrackingCache(GameObject gameObject)
        {
            if (!gameObject) return;
            _activeGameObject = gameObject;
            _serializedActiveGameObject = new SerializedObject(_activeGameObject);
            _serializedTransform = SerializeTransform(_activeGameObject);
            _trackedComponents = SerializeComponents();
        }

        private void ClearTrackingCache()
        {
            _activeGameObject = null;
            _serializedTransform = null;
            _trackedComponents.Clear();
            _trackedProperties.Clear();
            _trackedTransformProperties.Clear();
        }

        private void RebuildTrackingCache(GameObject gameObject)
        {
            ClearTrackingCache();
            BuildTrackingCache(gameObject);
        }

        private void StopTracking()
        {
            EditorApplication.update -= OnEditorApplicationUpdate;
        }

        private List<SerializedObject> SerializeComponents()
        {
            return _activeGameObject?.GetComponents<Component>()
                .Where(component => TrackComponentTypes.Contains(component.GetType()))
                .Select(component => component switch
                {
                    SpriteRenderer spriteRenderer => SerializeSpriteRenderer(spriteRenderer),
                    Camera camera => SerializeCamera(camera),
                    _ => new SerializedObject(component)
                })
                .ToList();
        }

        private SerializedObject SerializeTransform(GameObject gameObject)
        {
            var trackedPosition = new T2DTrackedProperty("position", () => gameObject.transform.position);
            var trackedScale = new T2DTrackedProperty("localScale", () => gameObject.transform.localScale);
            var trackedRotation = new T2DTrackedProperty("rotation", () => gameObject.transform.rotation);
            _trackedTransformProperties.Add("position", trackedPosition);
            _trackedTransformProperties.Add("scale", trackedScale);
            _trackedTransformProperties.Add("rotation", trackedRotation);
            return new SerializedObject(gameObject.transform);
        }

        private SerializedObject SerializeSpriteRenderer(SpriteRenderer renderer)
        {
            if (!renderer) return null;

            _trackedProperties.Add(new T2DTrackedProperty("sprite", () => renderer.sprite));

            if (!renderer.sprite) return new SerializedObject(renderer);
            _trackedProperties.Add(new T2DTrackedProperty("sprite.pivot", () => renderer.sprite?.pivot));
            _trackedProperties.Add(new T2DTrackedProperty("sprite.pixelsPerUnit",
                () => renderer.sprite?.pixelsPerUnit));
            return new SerializedObject(renderer);
        }

        private SerializedObject SerializeCamera(Camera camera)
        {
            if (!camera) return null;
            _trackedProperties.Add(new T2DTrackedProperty("camera.orthographic", () => camera.orthographic));
            _trackedProperties.Add(new T2DTrackedProperty("camera.aspect", () => camera.aspect));
            return new SerializedObject(camera);
        }

        private void OnEditorApplicationUpdate()
        {
            if (Is2DMode != _is2DMode)
            {
                _is2DMode = Is2DMode;
                DispatchChange(T2DEditorEventType.SceneViewMode, _is2DMode);
                return;
            }

            if (!_activeGameObject) return;

            if (GameObjectHasChanged)
            {
                RebuildTrackingCache(_activeGameObject);
                DispatchChange(T2DEditorEventType.GameObject);
            }

            else if (TransformHasChanged)
            {
                var transformType =
                    _trackedTransformProperties["rotation"].HasChanged ? T2DEditorTransformEventType.Rotate
                    : _trackedTransformProperties["scale"].HasChanged ? T2DEditorTransformEventType.Resize
                    : _trackedTransformProperties["position"].HasChanged ? T2DEditorTransformEventType.Move
                    : T2DEditorTransformEventType.None;

                DispatchChange(T2DEditorEventType.Transform, transformType);
            }

            else
            {
                var modifiedProperty = GetFirstModifiedTrackedProperty();
                if (modifiedProperty != null)
                {
                    if (modifiedProperty is "sprite" or "camera.orthographic")
                        RebuildTrackingCache(_activeGameObject);
                    DispatchChange(T2DEditorEventType.TrackedProperty, modifiedProperty);
                }
                else if (TrackedComponentHaveChanged)
                    DispatchChange(T2DEditorEventType.TrackedComponent);
            }
        }

        public static bool Is2DMode => SceneView.lastActiveSceneView != null && SceneView.lastActiveSceneView.in2DMode;
    }

    public class T2DTrackedProperty
    {
        private object _cachedValue;
        private readonly Func<object> _getValue;

        public string Name { get; }

        public T2DTrackedProperty(string propertyName, Func<object> getValue)
        {
            Name = propertyName;
            _getValue = getValue;
            _cachedValue = _getValue();
        }

        public bool HasChanged
        {
            set
            {
                if (value == false) _cachedValue = _getValue();
            }
            get
            {
                var currentValue = _getValue();
                if (EqualityComparer<object>.Default.Equals(currentValue, _cachedValue)) return false;
                _cachedValue = currentValue;
                return true;
            }
        }
    }
}
