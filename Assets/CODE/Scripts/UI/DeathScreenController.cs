using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreenController : MonoBehaviour
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
        if (GameInput.RestartPressedThisFrame && GameState.IsPaused && GameState.IsPlayerDead)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            gameObject.SetActive(false);
            Time.timeScale = 1;
            GameState.ResetForGameplay();
            mainUI.SetActive(true);
            MusicManager.instance.SaveMusicTime();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
