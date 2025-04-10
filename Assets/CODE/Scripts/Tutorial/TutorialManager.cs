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
    public static bool TutActive = false;

    public GameObject mainUI;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {

        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Tut1" && !Tut1Done)
        {
            tutorialScreens[0].SetActive(true);
            PlayerMovementAdvanced.Paused = true;
            Time.timeScale = 0;
            mainUI.SetActive(false);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Tut1Done = true;
            TutActive = true;
        }
        else if (other.tag == "Tut2" && !Tut2Done)
        {
            tutorialScreens[1].SetActive(true);
            PlayerMovementAdvanced.Paused = true;
            Time.timeScale = 0;
            mainUI.SetActive(false);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Tut2Done = true;
            TutActive = true;
        }
        else if (other.tag == "Tut3" && !Tut3Done)
        {
            tutorialScreens[2].SetActive(true);
            PlayerMovementAdvanced.Paused = true;
            Time.timeScale = 0;
            mainUI.SetActive(false);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Tut3Done = true;
            TutActive = true;
        }
        else if (other.tag == "Tut4" && !Tut4Done)
        {
            tutorialScreens[3].SetActive(true);
            PlayerMovementAdvanced.Paused = true;
            Time.timeScale = 0;
            mainUI.SetActive(false);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Tut4Done = true;
            TutActive = true;
        }
        else if (other.tag == "Tut5" && !Tut5Done)
        {
            tutorialScreens[4].SetActive(true);
            PlayerMovementAdvanced.Paused = true;
            Time.timeScale = 0;
            mainUI.SetActive(false);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Tut5Done = true;
            TutActive = true;
        }
        else if (other.tag == "Tut6" && !Tut6Done)
        {
            tutorialScreens[5].SetActive(true);
            PlayerMovementAdvanced.Paused = true;
            Time.timeScale = 0;
            mainUI.SetActive(false);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Tut6Done = true;
            TutActive = true;
        }
    }

    public void CloseWindow()
    {
        tutorialScreens[0].SetActive(false);
        tutorialScreens[1].SetActive(false);
        tutorialScreens[2].SetActive(false);
        tutorialScreens[3].SetActive(false);
        tutorialScreens[4].SetActive(false);
        tutorialScreens[5].SetActive(false);

        PlayerMovementAdvanced.Paused = false;
        Time.timeScale = 1;
        mainUI.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        TutActive = false;
    }
}
