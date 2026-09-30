using UnityEngine;

/// <summary>
/// Tracks glue-tagged objects without FindGameObjectsWithTag on every wall-run start.
/// </summary>
public static class GlueRegistry
{
    private static GameObject[] _cache = System.Array.Empty<GameObject>();
    private static bool _dirty = true;

    public static void MarkDirty() => _dirty = true;

    public static bool HasActiveGlue()
    {
        if (_dirty)
        {
            _cache = GameObject.FindGameObjectsWithTag("glue");
            _dirty = false;
        }

        for (int i = 0; i < _cache.Length; i++)
        {
            GameObject glue = _cache[i];
            if (glue != null && glue.activeInHierarchy)
                return true;
        }

        return false;
    }
}
