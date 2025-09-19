//----------------------------------------------
// Transform2D
// Copyright © 2024 BaconOppenheim™
//----------------------------------------------

using tools.bnop.Transform2d.Data;
using tools.bnop.Transform2d.Service;
using UnityEditor;
using UnityEngine;

namespace tools.bnop.Transform2d.View
{
    public partial class T2DView
    {
        private readonly Vector2Cache _sizeCache = new();
        private readonly Vector2Cache _scaleCache = new();

        private void RenderTransform()
        {
            EditorGUILayout.BeginHorizontal(Styles.TransformSection);
            {
                RenderPositionPivotGUI();
                Space(16);
                EditorGUILayout.BeginVertical(Styles.TransformFields);

                DisableIf(_data.ObjectCount == 0, () =>
                {

                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.BeginHorizontal();

                    _data.Transform.Position =
                        Vector2Field(_data.Transform.Position, _data.Settings.UnitType);

                    Space();

                    MoreMenu(PositionContextMenu);

                    EditorGUILayout.EndHorizontal();

                    if (EditorGUI.EndChangeCheck())
                        DispatchTransformChange(T2DViewChangeEventType.Position);

                    Space(3);


                    EditorGUILayout.BeginHorizontal();
                    DisableIf(!_data.SizeIsEditable, () =>
                    {
                        EditorGUI.BeginChangeCheck();
                        var prevSize = _data.Transform.Size;
                        _data.Transform.Size =
                            Vector2Field(new[] { "W", "H" }, _data.Transform.Size, _data.Settings.UnitType);

                        if (EditorGUI.EndChangeCheck())
                        {
                            if (_data.Settings.ConstrainProportions)
                                _data.Transform.Size = ConstrainProportions(prevSize, _data.Transform.Size, _sizeCache);

                            DispatchTransformChange(T2DViewChangeEventType.Size);
                        }
                    });

                    Space();

                    MoreMenu(SizeContextMenu);

                    EditorGUILayout.EndHorizontal();

                    Space(3);


                    EditorGUILayout.BeginHorizontal();
                    DisableIf(!_data.ScaleIsEditable, () =>
                    {
                        EditorGUI.BeginChangeCheck();

                        var prevScale = _data.Transform.Scale;

                        _data.Transform.Scale =
                            UnitVector2Field(new[] { "W", "H" }, _data.Transform.Scale * 100,
                                T2DSettingsData.FloatPrecisionPixels, Icons.Percent) / 100;

                        if (EditorGUI.EndChangeCheck())
                        {
                            if (_data.Settings.ConstrainProportions)
                            {
                                _data.Transform.Scale =
                                    ConstrainProportions(prevScale, _data.Transform.Scale, _scaleCache);
                            }

                            DispatchTransformChange(T2DViewChangeEventType.Scale);
                        }
                    });

                    Space();

                    MoreMenu(ScaleContextMenu);

                    EditorGUILayout.EndHorizontal();


                    Space(3);


                    EditorGUILayout.BeginHorizontal();
                    DisableIf(!_data.RotationIsEditable, () =>
                    {
                        EditorGUI.BeginChangeCheck();

                        _data.Transform.Rotation =
                            UnitFloatField("R", _data.Transform.Rotation,
                                T2DSettingsData.FloatPrecisionPixels, Icons.Degrees);


                        if (EditorGUI.EndChangeCheck())
                            DispatchTransformChange(T2DViewChangeEventType.Rotation);
                    });
                    
                    Space(10);
                    Invisible(() =>
                    {
                        UnitFloatField(" ", 0,
                            T2DSettingsData.FloatPrecisionUnits, Icons.Degrees);
                    });

                    Space();

                    MoreMenu(RotationContextMenu);

                    EditorGUILayout.EndHorizontal();
                });

                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
        }


        private GenericMenu PositionContextMenu
        {
            get
            {
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Copy"), false,
                    () => { T2DClipboard.Set(_data.Transform.Position); });

                if (T2DClipboard.Has<Vector2>())
                    menu.AddItem(new GUIContent("Paste"), false, () =>
                    {
                        _data.Transform.Position = T2DClipboard.Get<Vector2>();
                        DispatchTransformChange(T2DViewChangeEventType.Position);
                    });
                else
                    menu.AddDisabledItem(new GUIContent("Paste"), false);
                return menu;
            }
        }

        private GenericMenu SizeContextMenu
        {
            get
            {
                var menu = new GenericMenu();

                if (_data.SizeIsEditable)
                    menu.AddItem(new GUIContent("Constrain Proportions"), _data.Settings.ConstrainProportions,
                        () =>
                        {
                            _data.Settings.ConstrainProportions = !_data.Settings.ConstrainProportions;
                            DispatchSettingsChange();
                        });
                else
                    menu.AddDisabledItem(new GUIContent("Constrain Proportions"), _data.Settings.ConstrainProportions);

                if (!_data.SizeIsEditable || _data.Transform.Size == _data.UnscaledSize)
                    menu.AddDisabledItem(new GUIContent("Reset Size"), false);
                else
                    menu.AddItem(new GUIContent("Reset Size"), false, () =>
                    {
                        _data.Transform.Size =
                            _data.Settings.UnitType == UnitType.Pixels
                                ? T2DScreen.ToPixels(_data.UnscaledSize)
                                : _data.UnscaledSize;
                        DispatchTransformChange(T2DViewChangeEventType.Size);
                    });

                menu.AddSeparator("");

                menu.AddItem(new GUIContent("Copy"), false, () => { T2DClipboard.Set(_data.Transform.Size); });

                if (_data.SizeIsEditable && T2DClipboard.Has<Vector2>())
                    menu.AddItem(new GUIContent("Paste"), false, () =>
                    {
                        _data.Transform.Size = T2DClipboard.Get<Vector2>();
                        DispatchTransformChange(T2DViewChangeEventType.Size);
                    });
                else
                    menu.AddDisabledItem(new GUIContent("Paste"), false);

                return menu;
            }
        }

        private GenericMenu ScaleContextMenu
        {
            get
            {
                var menu = new GenericMenu();

                if (_data.ScaleIsEditable)
                    menu.AddItem(new GUIContent("Constrain Proportions"), _data.Settings.ConstrainProportions,
                        () =>
                        {
                            _data.Settings.ConstrainProportions = !_data.Settings.ConstrainProportions;
                            DispatchSettingsChange();
                        });
                else
                    menu.AddDisabledItem(new GUIContent("Constrain Proportions"), _data.Settings.ConstrainProportions);

                if (!_data.ScaleIsEditable || _data.Transform.Scale == Vector2.one)
                    menu.AddDisabledItem(new GUIContent("Reset Scale"), false);
                else
                    menu.AddItem(new GUIContent("Reset Scale"), false, () =>
                    {
                        _data.Transform.Scale = Vector2.one;
                        DispatchTransformChange(T2DViewChangeEventType.Scale);
                    });

                menu.AddSeparator("");

                menu.AddItem(new GUIContent("Copy"), false, () => { T2DClipboard.Set(_data.Transform.Scale); });

                if (_data.ScaleIsEditable && T2DClipboard.Has<Vector2>())
                    menu.AddItem(new GUIContent("Paste"), false, () =>
                    {
                        _data.Transform.Scale = T2DClipboard.Get<Vector2>();
                        DispatchTransformChange(T2DViewChangeEventType.Scale);
                    });
                else
                    menu.AddDisabledItem(new GUIContent("Paste"), false);

                return menu;
            }
        }

        private GenericMenu RotationContextMenu
        {
            get
            {
                var menu = new GenericMenu();

                if (!_data.RotationIsEditable || _data.Transform.Rotation == 0)
                    menu.AddDisabledItem(new GUIContent("Reset Rotation"), false);
                else
                    menu.AddItem(new GUIContent("Reset Rotation"), false, () =>
                    {
                        _data.Transform.Rotation = 0;
                        DispatchTransformChange(T2DViewChangeEventType.Rotation);
                    });
                
                menu.AddSeparator("");

                menu.AddItem(new GUIContent("Copy"), false, () => { T2DClipboard.Set(_data.Transform.Rotation); });

                if (_data.RotationIsEditable && T2DClipboard.Has<float>())
                    menu.AddItem(new GUIContent("Paste"), false, () =>
                    {
                        _data.Transform.Rotation = T2DClipboard.Get<float>();
                        DispatchTransformChange(T2DViewChangeEventType.Rotation);
                    });
                else
                    menu.AddDisabledItem(new GUIContent("Paste"), false);
                
                return menu;
            }
        }

        private void RenderPositionPivotGUI()
        {
            var enabled = _data.ActiveBoundsSource != BoundsSource.None;

            if (enabled)
            {
                var pivotsLayout = new[]
                {
                    PivotType.TopLeft,
                    PivotType.TopCenter,
                    PivotType.TopRight,
                    PivotType.LeftCenter,
                    PivotType.Center,
                    PivotType.RightCenter,
                    PivotType.BottomLeft,
                    PivotType.BottomCenter,
                    PivotType.BottomRight,
                };

                var rect = EditorGUILayout.BeginVertical(Styles.PivotWidget);
                for (var i = 0; i < 3; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    for (var j = 0; j < 3; j++)
                    {
                        var pivot = pivotsLayout[i * 3 + j];
                        if (GUILayout.Button(new GUIContent("", $"Use {Pivot.NameOf(pivot)} Anchor"),
                                pivot == _data.Settings.PivotType
                                    ? Styles.PivotWidgetButtonActive
                                    : Styles.PivotWidgetButton))
                        {
                            GUI.FocusControl(null);
                            _data.Settings.PivotType = pivot;
                            DispatchSettingsChange();
                        }
                    }

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.EndVertical();

                if (Event.current.type == EventType.MouseMove && rect.Contains(Event.current.mousePosition))
                    RepaintRequest?.Invoke();
            }
            else
            {
                var prevIconSize = EditorGUIUtility.GetIconSize();
                EditorGUIUtility.SetIconSize(Vector2.one * StyleProperties.PivotWidgetSize);
                GUILayout.Label(new GUIContent()
                    {
                        image = Image(Backgrounds.PivotWidgetDisabled),
                        tooltip = "Using No Bounds"
                    },
                    new GUIStyle(Styles.PivotWidget)
                    {
                        padding = UniformRectOffset(0),
                        normal =
                        {
                            background = null
                        }
                    }
                );
                EditorGUIUtility.SetIconSize(prevIconSize);
            }
        }
    }
}
