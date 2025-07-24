using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;

public class MockMenuSelectLevel : MonoBehaviour
{
    public GameObject panelSelectLevel, panelOptions;
    Renderer rend;
    public Material[] material;

    private float timer = 0f;
    private bool isOnState = true;
    public static bool isMouseOver = false;

    public GameObject OptionsButton;
    public GameObject LevelButton;
    public GameObject QuitButton;
    public GameObject StartButton;

    private PostProcessVolume postProcessVolume;
    private DepthOfField depthOfField;
    public Camera mainCamera;
    
    // Flickering interval in seconds (adjust to control flicker speed)
    public float flickerInterval = 0.2f;
    private GameObject credits;
    // Start is called before the first frame update
    void Start()
    {
        // Initialize flickering by setting all lights to "off" (material[0]) initially
        int numOfChildren = transform.childCount;
        for (int i = 0; i < numOfChildren; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            rend = child.GetComponent<Renderer>();
            rend.enabled = true;
            rend.sharedMaterial = material[0];
        }

        // Find the post-process volume attached to the Main Camera
        postProcessVolume = mainCamera.GetComponent<PostProcessVolume>();
        postProcessVolume.profile.TryGetSettings(out depthOfField);
        credits = GameObject.FindGameObjectWithTag("Credits");

    }

    // Update is called once per frame
    void Update()
    {
        // Flickering logic runs if the panel is active and the mouse is not over the object
        if (!isMouseOver)
        {
            // Increment the timer by the time elapsed since the last frame
            timer += Time.deltaTime;

            // Check if the flicker interval has passed
            if (timer >= flickerInterval)
            {
                // Toggle the state and reset the timer
                isOnState = !isOnState;
                timer = 0f;

                int numOfChildren = transform.childCount;

                for (int i = 0; i < numOfChildren; i++)
                {
                    GameObject child = transform.GetChild(i).gameObject;
                    Renderer rend = child.GetComponent<Renderer>();

                    // Set the material based on the alternating pattern and state
                    if ((i % 2 == 0 && isOnState) || (i % 2 != 0 && !isOnState))
                    {
                        rend.sharedMaterial = material[1];  // Turn on
                    }
                    else
                    {
                        rend.sharedMaterial = material[0];  // Turn off
                    }
                }
            }
        }
    }

    private void OnMouseUpAsButton()
    {
        if (panelSelectLevel.gameObject.activeSelf == false && panelOptions.gameObject.activeSelf == false)
        {
            // Open the select level panel
            panelSelectLevel.gameObject.SetActive(true);
            LevelButton.GetComponent<MockMenuSelectLevel>().enabled = false;
            LevelButton.GetComponent<Collider>().enabled = false;
            QuitButton.GetComponent<MockMenuExit>().enabled = false;
            QuitButton.GetComponent<Collider>().enabled = false;
            StartButton.GetComponent<MockMenuStartGame>().enabled = false;
            StartButton.GetComponent<Collider>().enabled = false;
            OptionsButton.GetComponent<MockMenuOptions>().enabled = false;
            OptionsButton.GetComponent<Collider>().enabled = false;

            EnableBlur();
            credits.SetActive(false);
        }
    }

    private void OnMouseEnter()
    {
        if (panelSelectLevel.gameObject.activeSelf == false && panelOptions.gameObject.activeSelf == false)
        {
            // Stop flickering and set all lights to on
            isMouseOver = true;

            int numOfChildren = transform.childCount;
            for (int i = 0; i < numOfChildren; i++)
            {
                GameObject child = transform.GetChild(i).gameObject;
                rend = child.GetComponent<Renderer>();
                rend.enabled = true;
                rend.sharedMaterial = material[1];
            }
        }
    }

    private void OnMouseExit()
    {
        if (panelSelectLevel.gameObject.activeSelf == false && panelOptions.gameObject.activeSelf == false)
        {
            // Resume flickering
            isMouseOver = false;
        }
    }

    public void EnableBlur()
    {
        depthOfField.active = true;

    }

    public void DisableBlur()
    {
        depthOfField.active = false;
    }
}
