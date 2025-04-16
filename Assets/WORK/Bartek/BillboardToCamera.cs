using UnityEngine;

public class BillboardToCamera : MonoBehaviour
{
    public Transform player; // Assign this in the Inspector or dynamically

    void LateUpdate()
    {
        if (player != null)
        {
            transform.LookAt(player);
            transform.rotation = Quaternion.LookRotation(transform.position - player.position);
        }
    }
}
