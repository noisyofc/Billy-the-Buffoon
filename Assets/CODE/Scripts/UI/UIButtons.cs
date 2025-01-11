using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButtons : MonoBehaviour
{
    public GameObject mouse0;
    public GameObject mouse1;
    public GameObject shift;
    public GameObject space;

    public GameObject RT;
    public GameObject X;
    public GameObject RB;
    public GameObject LT;

    void Update()
    {
        foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(key))
            {
                mouse0.SetActive(true);
                mouse1.SetActive(true);
                shift.SetActive(true);
                space.SetActive(true);

                RT.SetActive(false);
                X.SetActive(false);
                RB.SetActive(false);
                LT.SetActive(false);
            }
        }

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        if (Mathf.Abs(mouseX) > 0.1f || Mathf.Abs(mouseY) > 0.1f)
        {
            mouse0.SetActive(true);
            mouse1.SetActive(true);
            shift.SetActive(true);
            space.SetActive(true);

            RT.SetActive(false);
            X.SetActive(false);
            RB.SetActive(false);
            LT.SetActive(false);
        }

        for (int i = 0; i < 20; i++)
        {
            if (Input.GetKeyDown((KeyCode)((int)KeyCode.JoystickButton0 + i)))
            {
                mouse0.SetActive(false);
                mouse1.SetActive(false);
                shift.SetActive(false);
                space.SetActive(false);

                RT.SetActive(true);
                X.SetActive(true);
                RB.SetActive(true);
                LT.SetActive(true);
            }
        }

        string[] axes = { "Horizontalp", "Verticalp", "Tramp", "Parasol" };
        foreach (string axis in axes)
        {
            float value = Input.GetAxis(axis);
            if (Mathf.Abs(value) > 0.1f)
            {
                mouse0.SetActive(false);
                mouse1.SetActive(false);
                shift.SetActive(false);
                space.SetActive(false);

                RT.SetActive(true);
                X.SetActive(true);
                RB.SetActive(true);
                LT.SetActive(true);
            }
        }
    }
}
