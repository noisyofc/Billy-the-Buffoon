using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonEscape : MonoBehaviour
{
    [Header("Button, kt�ry ma by� wywo�any")]
    public Button button;

    void Update()
    {
#if UNITY_WEBGL
        if (Input.GetButtonDown("Cancel") && !EndScreen.endLevel)
#endif
#if !UNITY_WEBGL
        if (Input.GetButtonDown("Pause") && !EndScreen.endLevel)
#endif
        {
            if (button.interactable)
            {
                button.onClick.Invoke();
            }
        }
    }
}
