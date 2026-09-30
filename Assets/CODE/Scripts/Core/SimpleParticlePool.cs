using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tiny per-prefab particle pool to avoid Instantiate spam from projectiles.
/// </summary>
public static class SimpleParticlePool
{
    private static readonly Dictionary<int, Queue<GameObject>> Pools = new Dictionary<int, Queue<GameObject>>();

    public static void Play(GameObject prefab, Vector3 position, Quaternion rotation, MonoBehaviour runner, float fallbackLifetime = 2f)
    {
        if (prefab == null || runner == null)
            return;

        GameObject instance = Get(prefab);
        instance.transform.SetPositionAndRotation(position, rotation);
        instance.SetActive(true);

        float duration = fallbackLifetime;
        ParticleSystem ps = instance.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            var main = ps.main;
            duration = main.duration + main.startLifetime.constantMax;
            ps.Clear(true);
            ps.Play(true);
        }

        runner.StartCoroutine(ReturnAfter(prefab, instance, duration));
    }

    public static void PlayAll(GameObject[] prefabs, Vector3 position, Quaternion rotation, MonoBehaviour runner)
    {
        if (prefabs == null)
            return;

        for (int i = 0; i < prefabs.Length; i++)
            Play(prefabs[i], position, rotation, runner);
    }

    private static GameObject Get(GameObject prefab)
    {
        int id = prefab.GetInstanceID();
        if (!Pools.TryGetValue(id, out Queue<GameObject> queue))
        {
            queue = new Queue<GameObject>();
            Pools[id] = queue;
        }

        while (queue.Count > 0)
        {
            GameObject pooled = queue.Dequeue();
            if (pooled != null)
                return pooled;
        }

        return Object.Instantiate(prefab);
    }

    private static IEnumerator ReturnAfter(GameObject prefab, GameObject instance, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (instance == null)
            yield break;

        instance.SetActive(false);
        int id = prefab.GetInstanceID();
        if (!Pools.TryGetValue(id, out Queue<GameObject> queue))
        {
            queue = new Queue<GameObject>();
            Pools[id] = queue;
        }
        queue.Enqueue(instance);
    }
}
