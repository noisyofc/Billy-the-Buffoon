using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomImage : MonoBehaviour
{
    public Sprite[] imagePool;

    private Image imageComponent;

    void Start()
    {
        imageComponent = GetComponent<Image>();

        if (imagePool.Length > 0)
        {
            int randomIndex = Random.Range(0, imagePool.Length);
            imageComponent.sprite = imagePool[randomIndex];
        }
        else
        {
            Debug.LogWarning("Lista imagePool jest pusta!");
        }
    }
}
