using System.Collections;
using UnityEngine;

public class ChangeTagOfWall : MonoBehaviour
{
    [Header("Tag Settings")]
    public string gluedTag = "glued";
    public string defaultTag = "Wall";

    [Header("Countdown Settings")]
    public float resetTime = 1f;

    private bool _resetStarted;

    private void Start()
    {
        gameObject.tag = defaultTag;
    }

    private void Update()
    {
        if (_resetStarted || !CompareTag(gluedTag))
            return;

        _resetStarted = true;
        StartCoroutine(ResetTagAfterDelay());
    }

    private IEnumerator ResetTagAfterDelay()
    {
        yield return new WaitForSeconds(resetTime);

        gameObject.tag = defaultTag;
        GlueRegistry.MarkDirty();

        if (transform.childCount >= 2)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
        }

        _resetStarted = false;
    }
}
