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

    public void LoadTestLevel()
    {
        SceneManager.LoadScene("Scenes/Level 0 - TEST_LEVEL");
        Time.timeScale = 1;
    }
    public void StartGame()
    {
        SceneManager.LoadScene("Scenes/Level 1 - Borys");
        Time.timeScale = 1;
    }
    public void LoadGame()
    {
        SceneManager.LoadScene("Scenes/Level 3 - Borys");
        Time.timeScale = 1;
    }

    public void NextLevel()
    {
        SceneManager.LoadScene("Scenes/Level 2 - Borys");
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