using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


public class TutorialManager : MonoBehaviour
{

    [Header("Tutorial Screens")]
    [SerializeField] private GameObject[] tutorialScreens;
    public bool Tut1Done = false;
    public bool Tut2Done = false;
    public bool Tut3Done = false;
    public bool Tut4Done = false;
    public bool Tut5Done = false;
    public bool Tut6Done = false;
    public static bool TutActive
    {
        get => GameState.IsTutorialActive;
        set => GameState.IsTutorialActive = value;
    }

    public GameObject mainUI;

    void Update()
    {
        // reserved for tutorial skip input
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tut1") && !Tut1Done)
        {
            ShowTutorial(0);
            Tut1Done = true;
        }
        else if (other.CompareTag("Tut2") && !Tut2Done)
        {
            ShowTutorial(1);
            Tut2Done = true;
        }
        else if (other.CompareTag("Tut3") && !Tut3Done)
        {
            ShowTutorial(2);
            Tut3Done = true;
        }
        else if (other.CompareTag("Tut4") && !Tut4Done)
        {
            ShowTutorial(3);
            Tut4Done = true;
        }
        else if (other.CompareTag("Tut5") && !Tut5Done)
        {
            ShowTutorial(4);
            Tut5Done = true;
        }
        else if (other.CompareTag("Tut6") && !Tut6Done)
        {
            ShowTutorial(5);
            Tut6Done = true;
        }
    }

    void ShowTutorial(int index)
    {
        if (tutorialScreens == null || index < 0 || index >= tutorialScreens.Length)
            return;

        tutorialScreens[index].SetActive(true);
        GameState.IsPaused = true;
        GameState.IsTutorialActive = true;
        Time.timeScale = 0;
        if (mainUI != null) mainUI.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseWindow()
    {
        if (tutorialScreens != null)
        {
            for (int i = 0; i < tutorialScreens.Length; i++)
            {
                if (tutorialScreens[i] != null)
                    tutorialScreens[i].SetActive(false);
            }
        }

        GameState.IsPaused = false;
        GameState.IsTutorialActive = false;
        Time.timeScale = 1;
        if (mainUI != null) mainUI.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
