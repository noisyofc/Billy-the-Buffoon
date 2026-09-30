using UnityEngine;

public class Orientation : MonoBehaviour
{
    [SerializeField] private Transform orientator;
    [SerializeField] private Transform player;

    void Start()
    {
        if (orientator == null)
        {
            GameObject orientationGo = GameObject.Find("Orientation");
            if (orientationGo != null)
                orientator = orientationGo.transform;
        }

        if (player == null)
        {
            GameObject playerGo = GameObject.FindGameObjectWithTag("Player");
            if (playerGo == null)
                playerGo = GameObject.Find("Player");
            if (playerGo != null)
                player = playerGo.transform;
        }

        if (orientator != null && player != null)
            orientator.position = player.position;
    }
}
