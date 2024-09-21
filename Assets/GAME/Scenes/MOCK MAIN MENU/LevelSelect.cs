using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelect : MonoBehaviour
{

    public GameObject levelSelectPanel, optionsPanel;

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
    public void Level1()
    {
        SceneManager.LoadScene("Scenes/Level 1 - Borys");
        Time.timeScale = 1;
    }
    public void Level2()
    {
        SceneManager.LoadScene("Scenes/Level 2 - Borys");
        Time.timeScale = 1;
    }

    public void Level3()
    {
        SceneManager.LoadScene("Scenes/Level 3 - Borys");
        Time.timeScale = 1;
    }

    public void Level4()
    {
        Debug.Log("LOCKED");
    }

    public void goBack()
    {
        levelSelectPanel.SetActive(false);
    }

    public void goBackOptions()
    {
        optionsPanel.SetActive(false);
    }

}
