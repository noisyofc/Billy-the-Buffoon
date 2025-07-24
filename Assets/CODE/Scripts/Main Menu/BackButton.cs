using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class BackButton : MonoBehaviour
{

    public GameObject OptionsButton;
    public GameObject LevelButton;
    public GameObject QuitButton;
    public GameObject StartButton;

    public GameObject panelOptions;

    private PostProcessVolume postProcessVolume;
    private DepthOfField depthOfField;
    public Camera mainCamera;
    private GameObject credits;
    // Start is called before the first frame update
    void Start()
    {
        postProcessVolume = mainCamera.GetComponent<PostProcessVolume>();
        postProcessVolume.profile.TryGetSettings(out depthOfField);
        credits = GameObject.FindGameObjectWithTag("Credits");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void goBack()
    {
        //gameObject.SetActive(false);
        panelOptions.GetComponent<Canvas>().enabled = false;
        LevelButton.GetComponent<MockMenuSelectLevel>().enabled = true;
        LevelButton.GetComponent<Collider>().enabled = true;
        QuitButton.GetComponent<MockMenuExit>().enabled = true;
        QuitButton.GetComponent<Collider>().enabled = true;
        StartButton.GetComponent<MockMenuStartGame>().enabled = true;
        StartButton.GetComponent<Collider>().enabled = true;
        OptionsButton.GetComponent<MockMenuOptions>().enabled = true;
        OptionsButton.GetComponent<Collider>().enabled = true;
        MockMenuSelectLevel.isMouseOver = false;
        MockMenuOptions.isMouseOver = false;
        depthOfField.active = false;
        credits.SetActive(true);
    }
}
