using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

/// <summary>
/// Lightweight WebGL shipping hygiene (P4). Does not migrate to URP/Addressables.
/// </summary>
public class WebGlBuildPreprocessor : IPreprocessBuildWithReport
{
    public int callbackOrder => 10;

    public void OnPreprocessBuild(BuildReport report)
    {
        if (report.summary.platform != BuildTarget.WebGL)
            return;

#if UNITY_2021_2_OR_NEWER
        // Keep managed code stripping aggressive for download size.
        PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.WebGL, ManagedStrippingLevel.High);
#endif

        PlayerSettings.stripEngineCode = true;

        Debug.Log(
            "[Billy WebGL] Applied shipping defaults: stripEngineCode=true, managed stripping=High.\n" +
            "Still recommended manually: texture compression (DXT/ASTC via platform settings), " +
            "disable unused Build-in packages, bake static geometry, avoid Resources/ for large assets.");
    }

    [MenuItem("Billy/WebGL/Mark Selected Static For Batching")]
    public static void MarkSelectedStatic()
    {
        foreach (GameObject go in Selection.gameObjects)
        {
            GameObjectUtility.SetStaticEditorFlags(
                go,
                StaticEditorFlags.BatchingStatic |
                StaticEditorFlags.OccluderStatic |
                StaticEditorFlags.OccludeeStatic |
                StaticEditorFlags.ContributeGI);
            EditorUtility.SetDirty(go);
        }

        Debug.Log($"[Billy WebGL] Marked {Selection.gameObjects.Length} object(s) as static (batching/occlusion/GI).");
    }

    [MenuItem("Billy/WebGL/Open Addressables Migration Notes")]
    public static void AddressablesNotes()
    {
        EditorUtility.DisplayDialog(
            "Addressables (P4)",
            "Full Addressables migration is intentionally not auto-applied.\n\n" +
            "Current hook: SceneTransitionManager already uses LoadSceneAsync.\n" +
            "Next step when ready: move level scenes to Addressable groups and load by address/key instead of build-index.\n\n" +
            "URP migration is also a separate project (Built-in + Post Processing Stack v2 today).",
            "OK");
    }
}
