using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

/// <summary>
/// Blocks shipping builds that still include development/demo scenes,
/// and reports heavy non-shipping content folders.
/// </summary>
public class BuildContentGuard : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    private static readonly string[] BlockedScenePathFragments =
    {
        "/OLD ",
        "/OLD/",
        "[OLD]",
        "/Demo/",
        "/demo/",
        "SampleScene",
        "OLD BlockOut"
    };

    private static readonly string[] DevelopmentFolders =
    {
        "Assets/WORK",
        "Assets/ART/Models/[OLD] Imported models",
        "Assets/GAME/Scenes/OLD BlockOut",
        "Assets/Matthew Guz/Hits Effects FREE/Scene Demo",
        "Assets/msVFX_Free Smoke Effects Pack/Demo",
        "Assets/ART/Skybox/Skybox Cubemap Extended/Demo"
    };

    public void OnPreprocessBuild(BuildReport report)
    {
        var blocked = EditorBuildSettings.scenes
            .Where(s => s.enabled)
            .Where(s => BlockedScenePathFragments.Any(f => s.path.Contains(f)))
            .Select(s => s.path)
            .ToList();

        if (blocked.Count > 0)
        {
            throw new BuildFailedException(
                "Build blocked: development/demo scenes are enabled in Build Settings:\n- " +
                string.Join("\n- ", blocked));
        }

        var present = DevelopmentFolders.Where(AssetDatabase.IsValidFolder).ToList();
        if (present.Count > 0)
        {
            Debug.LogWarning(
                "[Billy Build] Development folders still in project (Editor-only bloat; OK if unreferenced):\n- " +
                string.Join("\n- ", present) +
                "\nUse menu Billy/Report Development Content Size for details.");
        }
    }

    [MenuItem("Billy/Report Development Content Size")]
    public static void ReportDevelopmentContent()
    {
        long total = 0;
        var lines = new List<string>();

        foreach (string folder in DevelopmentFolders)
        {
            if (!AssetDatabase.IsValidFolder(folder))
                continue;

            long bytes = FolderBytes(folder);
            total += bytes;
            lines.Add($"{folder} ≈ {FormatBytes(bytes)}");
        }

        Debug.Log(
            "[Billy] Development content report:\n" +
            string.Join("\n", lines) +
            $"\nTotal ≈ {FormatBytes(total)}\n" +
            "These assets are not in Build Settings; delete/move them when ready to shrink the repo.");
    }

    [MenuItem("Billy/Disable Demo Scenes In Build Settings")]
    public static void DisableDemoScenesInBuild()
    {
        var scenes = EditorBuildSettings.scenes;
        int disabled = 0;
        for (int i = 0; i < scenes.Length; i++)
        {
            if (!scenes[i].enabled)
                continue;
            if (!BlockedScenePathFragments.Any(f => scenes[i].path.Contains(f)))
                continue;

            scenes[i].enabled = false;
            disabled++;
        }

        EditorBuildSettings.scenes = scenes;
        Debug.Log($"[Billy] Disabled {disabled} demo/OLD scene(s) in Build Settings.");
    }

    private static long FolderBytes(string folder)
    {
        long sum = 0;
        string[] guids = AssetDatabase.FindAssets(string.Empty, new[] { folder });
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(path) || AssetDatabase.IsValidFolder(path))
                continue;
            sum += new System.IO.FileInfo(
                System.IO.Path.GetFullPath(path)).Exists
                ? new System.IO.FileInfo(System.IO.Path.GetFullPath(path)).Length
                : 0;
        }
        return sum;
    }

    private static string FormatBytes(long bytes)
    {
        if (bytes > 1024L * 1024L * 1024L)
            return $"{bytes / (1024f * 1024f * 1024f):0.00} GB";
        if (bytes > 1024L * 1024L)
            return $"{bytes / (1024f * 1024f):0.0} MB";
        return $"{bytes / 1024f:0} KB";
    }
}
