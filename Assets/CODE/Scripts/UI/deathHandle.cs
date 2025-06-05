using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class deathHandle : MonoBehaviour
{
    public GameObject mainUI;
    public string mainMenuScene = "GAME/Scenes/Menu";
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Restart") && PlayerMovementAdvanced.Paused==true && PlayerDeath.playerDead==true)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            gameObject.SetActive(false);
            Time.timeScale = 1;
            PlayerMovementAdvanced.Paused=false;
            PlayerDeath.playerDead=false;
            mainUI.SetActive(true);
            MusicManager.instance.SaveMusicTime();
            if (SceneManager.GetActiveScene().name != "Level_0_T")
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            
        }
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    public void MainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }
}
