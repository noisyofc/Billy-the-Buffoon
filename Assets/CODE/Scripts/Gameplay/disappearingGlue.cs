using System.Collections;
using UnityEngine;

public class DisappearingGlue : MonoBehaviour
{
    [Header("Disappearance Settings")]
    public float disappearanceTime = 2f;

    private void OnEnable()
    {
        GlueRegistry.MarkDirty();
    }

    private void Start()
    {
        StartCoroutine(CountdownToDisappear());
    }

    private void OnDisable()
    {
        GlueRegistry.MarkDirty();
    }

    IEnumerator CountdownToDisappear()
    {
        yield return new WaitForSeconds(disappearanceTime);
        Destroy(gameObject);
    }
}
