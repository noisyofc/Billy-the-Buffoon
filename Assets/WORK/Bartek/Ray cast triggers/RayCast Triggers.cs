using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastTriggers : MonoBehaviour
{
    public float spacing = 0.75f;     // Distance between each ray
    public float rayDistance = 10f;   // Length of each raycast

    public PlayerRespawnManager playerRespawnManager;

    void Update()
    {
        // Pole direction is local X (assuming length is along X axis)
        Vector3 poleDirection = transform.right;

        // Pole length in world units (along X axis)
        float poleLength = transform.localScale.x;

        // Number of rays to cast
        int numRays = Mathf.FloorToInt(poleLength / spacing) + 1;

        for (int i = 0; i < numRays; i++)
        {
            // Start from -halfLength and move to +halfLength
            float offset = -poleLength / 2f + i * spacing;

            // Calculate ray origin along the pole in world space
            Vector3 rayOrigin = transform.position + poleDirection * offset;

            // Cast upward
            if (Physics.Raycast(rayOrigin, Vector3.up, out RaycastHit hit, rayDistance))
            {
                Debug.DrawRay(rayOrigin, Vector3.up * hit.distance, Color.red);
                //Debug.Log($"Ray {i} hit: {hit.collider.name}");

                if (hit.collider.CompareTag("Player"))
                {
                    playerRespawnManager.currentRespawnPoint = gameObject.transform;
                    playerRespawnManager.lastResp = gameObject.name;
                    Debug.Log("HIT");
                }
            }
            else
            {
                Debug.DrawRay(rayOrigin, Vector3.up * rayDistance, Color.green);
            }
        }


    }
}
