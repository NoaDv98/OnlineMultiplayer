//----------------------------------------------
// Transform2D
// Copyright © 2024 BaconOppenheim™
//----------------------------------------------

using System;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace tools.bnop.Transform2d.Service
{
    public static class T2DClipboard
    {
        private static Match MatchFloat(string input)
        {
            return Regex.Match(input, @"^-?\d+(\.\d+)?$");
        }
        
        private static float ParseStringToFloat(string input)
        {
            var match = MatchFloat(input);
            if (match.Success)
            {
                return float.Parse(match.Value);
            }
            
            T2DConsole.LogError("Invalid float format");
            return 0f;
        }
        
        private const string Vector2Pattern = @"^Vector2\(\s*(-?\d+(\.\d+)?),\s*(-?\d+(\.\d+)?)\s*\)$";

        private static Match MatchVector2(string input) => Regex.Match(input, Vector2Pattern);
        
        private static Vector2 ParseStringToVector2(string input)
        {
            var matchVector2 = MatchVector2(input);
            if (matchVector2.Success)
            {
                var x = float.Parse(matchVector2.Groups[1].Value);
                var y = float.Parse(matchVector2.Groups[3].Value);

                return new Vector2(x, y);
            }
            
            if (MatchVector3(input).Success)
            {
                return ParseStringToVector3(input);
            }
            
            T2DConsole.LogError("Invalid Vector2 format");
            return Vector2.zero;
        }
        
        private static string Vector3Pattern = @"^Vector3\(\s*(-?\d+(\.\d+)?),\s*(-?\d+(\.\d+)?),\s*(-?\d+(\.\d+)?)\s*\)$";

        private static Match MatchVector3(string input) => Regex.Match(input, Vector3Pattern);

        private static Vector3 ParseStringToVector3(string input)
        {
            var match = MatchVector3(input);
            if (match.Success)
            {
                var x = float.Parse(match.Groups[1].Value);
                var y = float.Parse(match.Groups[3].Value);
                var z = float.Parse(match.Groups[5].Value);

                return new Vector3(x, y, z);
            }

            T2DConsole.LogError("Invalid Vector3 format");
            return Vector3.zero;
        }
        
        private static string SerializeFloat(float input)
        {
            return input.ToString();
        }

        private static string SerializeVector2(Vector2 input)
        {
            return $"Vector2({input.x.ToString()},{input.y.ToString()})";
        }
        
        private static string SerializeVector3(Vector3 input)
        {
            return $"Vector3({input.x.ToString()},{input.y.ToString()},{input.z.ToString()})";
        }
        
        public static void Set(string value) => EditorGUIUtility.systemCopyBuffer = value;
        public static void Set(float value) => Set(SerializeFloat(value));
        public static void Set(Vector2 value) => Set(SerializeVector2(value));
        public static void Set(Vector3 value) => Set(SerializeVector3(value));
        
        public static T Get<T>()
        {
            return typeof(T) switch
            {
                { } t when t == typeof(string) => (T)(object)EditorGUIUtility.systemCopyBuffer,
                { } t when t == typeof(float) => (T)(object)ParseStringToFloat(EditorGUIUtility.systemCopyBuffer),
                { } t when t == typeof(Vector2) => (T)(object)ParseStringToVector2(EditorGUIUtility.systemCopyBuffer),
                { } t when t == typeof(Vector3) => (T)(object)ParseStringToVector3(EditorGUIUtility.systemCopyBuffer),
                _ => throw new ArgumentException("Unsupported type")
            };
        }

        public static bool Is<T>()
        {
            return typeof(T) switch
            {
                { } t when t == typeof(string) => true,
                { } t when t == typeof(float) => MatchFloat(EditorGUIUtility.systemCopyBuffer).Success,
                { } t when t == typeof(Vector2) => MatchVector2(EditorGUIUtility.systemCopyBuffer).Success,
                { } t when t == typeof(Vector3) => MatchVector3(EditorGUIUtility.systemCopyBuffer).Success,
                _ => throw new ArgumentException("Unsupported type")
            };
        }
        
        public static bool Has<T>()
        {
            return typeof(T) switch
            {
                { } t when t == typeof(string) => true,
                { } t when t == typeof(float) => Is<float>(),
                { } t when t == typeof(Vector2) => Is<Vector2>() || Is<Vector3>(),
                { } t when t == typeof(Vector3) => Is<Vector2>() || Is<Vector3>(),
                _ => throw new ArgumentException("Unsupported type")
            };
        }
    }
}
