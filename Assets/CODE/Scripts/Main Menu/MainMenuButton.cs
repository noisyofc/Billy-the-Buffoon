using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

public class MainMenuButton : MonoBehaviour
{
    public string mainMenuScene = "GAME/Scenes/Menu";

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void goMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
        Time.timeScale = 1;
        PlayerMovementAdvanced.Paused = false;
    }
}
