using System.Diagnostics;
using UnityEngine;

/// <summary>
/// Player-facing logs are stripped from release builds.
/// Errors always remain.
/// </summary>
public static class GameLog
{
    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
    public static void Info(object message) => UnityEngine.Debug.Log(message);

    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
    public static void Info(object message, Object context) => UnityEngine.Debug.Log(message, context);

    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
    public static void Warn(object message) => UnityEngine.Debug.LogWarning(message);

    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
    public static void Warn(object message, Object context) => UnityEngine.Debug.LogWarning(message, context);

    public static void Error(object message) => UnityEngine.Debug.LogError(message);

    public static void Error(object message, Object context) => UnityEngine.Debug.LogError(message, context);
}
