using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Scenes/Level1");
        Time.timeScale = 1;
    }
    public void LoadGame()
    {
        SceneManager.LoadScene("Scenes/Level3");
        Time.timeScale = 1;
    }

    public void NextLevel()
    {
        SceneManager.LoadScene("Scenes/Level2");
        Time.timeScale = 1;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("Scenes/Main Menu");
    }
}
