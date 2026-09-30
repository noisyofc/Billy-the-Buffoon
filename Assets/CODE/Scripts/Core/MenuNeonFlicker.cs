using UnityEngine;

/// <summary>
/// Shared neon flicker helpers for main-menu mock buttons.
/// </summary>
public static class MenuNeonFlicker
{
    public static Renderer[] CacheChildRenderers(Transform root)
    {
        int count = root.childCount;
        var renderers = new Renderer[count];
        for (int i = 0; i < count; i++)
            renderers[i] = root.GetChild(i).GetComponent<Renderer>();
        return renderers;
    }

    public static void SetAll(Renderer[] renderers, Material material, bool enabled = true)
    {
        if (renderers == null || material == null)
            return;

        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer r = renderers[i];
            if (r == null)
                continue;
            r.enabled = enabled;
            r.sharedMaterial = material;
        }
    }

    public static void ApplyAlternating(Renderer[] renderers, Material onMat, Material offMat, bool isOnState)
    {
        if (renderers == null || onMat == null || offMat == null)
            return;

        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer r = renderers[i];
            if (r == null)
                continue;

            bool on = (i % 2 == 0 && isOnState) || (i % 2 != 0 && !isOnState);
            r.sharedMaterial = on ? onMat : offMat;
        }
    }
}
