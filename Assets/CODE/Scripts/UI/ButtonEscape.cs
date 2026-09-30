using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonEscape : MonoBehaviour
{
    [Header("Button do wywolania")]
    public Button button;
    private GameObject credits;

    void Start()
    {
        credits = GameObject.FindGameObjectWithTag("Credits");
    }

    void Update()
    {
        if (GameInput.PausePressedThisFrame && GameState.CanPause)
        {
            if (button != null && button.interactable)
                button.onClick.Invoke();
        }
    }
}
