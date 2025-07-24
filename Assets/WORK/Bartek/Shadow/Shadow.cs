using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour
{
    public Transform shadow;
    public LayerMask shadowLayer;
    public float maxDistance = 10f;
    public float minDistance = 0.5f;
    public float shadowYOffset = 0.01f; // prevent Z-fighting
    public float minScale = 0.5f;
    public float maxScale = 1f;
    public float maxHeight = 3f;

    void Update()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, maxDistance, shadowLayer))
        {
            // ✅ Ignore if too close
            if (hit.distance < minDistance)
            {
                shadow.gameObject.SetActive(false);
                return;
            }

            // ✅ Normal shadow logic
            Vector3 shadowPos = hit.point + hit.normal * shadowYOffset;
            shadow.position = shadowPos;
            shadow.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

            float height = hit.distance;
            float t = Mathf.Clamp01(height / maxHeight);
            float scale = Mathf.Lerp(maxScale, minScale, t);
            shadow.localScale = new Vector3(scale, 1, scale);

            shadow.gameObject.SetActive(true);
        }
        else
        {
            shadow.gameObject.SetActive(false);
        }
    }
}
