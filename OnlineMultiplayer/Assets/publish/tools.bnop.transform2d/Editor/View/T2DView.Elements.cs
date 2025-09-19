//----------------------------------------------
// Transform2D
// Copyright © 2024 BaconOppenheim™
//----------------------------------------------

using System;
using tools.bnop.Transform2d.Data;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace tools.bnop.Transform2d.View
{
    public partial class T2DView
    {
        private static int Toolbar(int currentValue, GUIContent[] contents,
            bool[] enabled = null,
            [CanBeNull] params GUILayoutOption[] options)
        {
            var selected = GUILayout.Toolbar(currentValue, contents, enabled, Styles.Toolbar, options);
            if (selected != currentValue)
            {
                GUI.FocusControl(null);
            }

            return selected;
        }

        private static void MoreMenu(GenericMenu menu)
        {
            if (!GUILayout.Button(Image(Icons.More), new GUIStyle(EditorStyles.iconButton)
                {
                    margin = UniformRectOffset(0, 2),
                    fixedWidth = 16,
                    fixedHeight = 16
                })) return;
            GUI.FocusControl(null);
            menu.ShowAsContext();
        }

        private static Vector2 Vector2Field(Vector2 value, UnitType unitType = UnitType.WorldUnits)
        {
            return Vector2Field(new[] { "X", "Y" }, value, unitType);
        }

        private static Vector2 Vector2Field(string[] labels, Vector2 value, UnitType unitType = UnitType.WorldUnits)
        {
            var precision = unitType == UnitType.WorldUnits
                ? T2DSettingsData.FloatPrecisionUnits
                : T2DSettingsData.FloatPrecisionPixels;
            var icon = unitType == UnitType.Pixels ? Icons.Pixels : Icons.Units;

            return UnitVector2Field(labels, value, precision, icon);
        }

        private static float CoordinateField(string label, float value, UnitType unitType = UnitType.WorldUnits,
            params GUILayoutOption[] options)
        {
            var precision = unitType == UnitType.WorldUnits
                ? T2DSettingsData.FloatPrecisionUnits
                : T2DSettingsData.FloatPrecisionPixels;
            var icon = unitType == UnitType.Pixels ? Icons.Pixels : Icons.Units;
            return UnitFloatField(label, value, precision, icon, options);
        }

        private static Vector2 UnitVector2Field(Vector2 value, int precision,
            string unitIcon)
        {
            return UnitVector2Field(new[] { "X", "Y" }, value, precision, unitIcon);
        }
        
        private static Vector2 UnitVector2Field(string[] labels, Vector2 value, int precision,
            string unitIcon, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal();
            var x = UnitFloatField(labels[0], value.x, precision, unitIcon, options);
            Space(10);
            var y = UnitFloatField(labels[1], value.y, precision, unitIcon, options);
            EditorGUILayout.EndHorizontal();
            return new Vector2(x, y);
        }

        private static float UnitFloatField(string label, float value, int precision, string unitIcon,
            params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal();

            var prevLabelWidth = EditorGUIUtility.labelWidth;
            var style = new GUIStyle(EditorStyles.numberField)
            {
                margin = UniformRectOffset(0),
                fixedHeight = 19,
            };

            EditorGUIUtility.labelWidth = 16;
            var displayValue = CleanFloat(value, precision);
            var inputValue = EditorGUILayout.FloatField(label, displayValue, style, options);
            if (!Mathf.Approximately(inputValue, displayValue))
                value = inputValue;
            EditorGUIUtility.labelWidth = prevLabelWidth;

            var prevColor = GUI.color;
            if (prevColor.a != 0)
                GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, 0.6f);
            GUILayout.Label(Image(unitIcon), new GUIStyle(EditorStyles.label)
            {
                fixedWidth = 16,
            });
            GUI.color = prevColor;

            EditorGUILayout.EndHorizontal();

            return value;
        }

        private static bool FoldoutSection(ref bool isOpen, string label, Action content)
        {
            var wasOpen = isOpen;
            EditorGUILayout.BeginVertical(Styles.BorderTop);
            isOpen = EditorGUILayout.BeginFoldoutHeaderGroup(isOpen, label);
            if (isOpen)
            {
                EditorGUILayout.BeginVertical(Styles.IndentedSection);
                content?.Invoke();
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.EndVertical();
            return isOpen != wasOpen;
        }


        private bool IconButton(GUIContent content)
        {
            var clicked = GUILayout.Button(content, new GUIStyle(EditorStyles.iconButton)
            {
                fixedHeight = 28,
                fixedWidth = 28,
                alignment = TextAnchor.MiddleCenter,
                hover =
                {
                    background = (Texture2D)Image(Backgrounds.IconButtonHovered)
                },
                active =
                {
                    background = (Texture2D)Image(Backgrounds.IconButtonActive)
                }
            });

            if (clicked)
            {
                GUI.FocusControl(null);
            }

            return clicked;
        }

        private Vector2 _scrollPosition;

        private void ScrollView(Action content)
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            content.Invoke();
            EditorGUILayout.EndScrollView();
        }

        private static void IndentedBorderedSection(Action content)
        {
            EditorGUILayout.BeginVertical(Styles.IndentedBorderedSection);
            content.Invoke();
            EditorGUILayout.EndVertical();
        }

        private static void Message(string message, MessageType type, string actionLabel = null,
            Action actionCallback = null)
        {
            var icon = type switch
            {
                MessageType.Warning => Icons.Warning,
                MessageType.Error => Icons.Error,
                _ => Icons.Info
            };
            EditorGUILayout.BeginHorizontal(Styles.Message);
            GUILayout.Label(Image(icon), new GUIStyle(EditorStyles.label)
            {
                stretchWidth = false,
                margin = new RectOffset(0, 8, 0, 0)
            });
            GUILayout.Label(message, new GUIStyle(EditorStyles.label)
            {
                wordWrap = true,
                margin = UniformRectOffset(0)
            });

            if (actionLabel != null & actionCallback != null)
            {
                if (GUILayout.Button(actionLabel, new GUIStyle(EditorStyles.miniButton)
                    {
                        margin = new RectOffset(8, 0, 0, 0),
                        fixedWidth = 40
                    }))
                {
                    actionCallback.Invoke();
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        private static void Space(int gap = 8)
        {
            EditorGUILayout.Space(gap, false);
        }
        
        private static void Invisible(Action content)
        {
            var prevColor = GUI.color;
            var prevEnabled = GUI.enabled;
            GUI.color = new Color(prevColor.r, prevColor.g, prevColor.b, 0f);
            GUI.enabled = false;
            content?.Invoke();
            GUI.color = prevColor;
            GUI.enabled = prevEnabled;
        }
    }
}
