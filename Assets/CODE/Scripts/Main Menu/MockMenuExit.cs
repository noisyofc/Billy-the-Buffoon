using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MockMenuExit : MonoBehaviour
{
    Renderer rend;
    public Material[] material;
    public GameObject panelSelectLevel, panelOptions;

    private float timer = 0f;
    private bool isOnState = true;
    private bool isMouseOver = false;
    
    // Flickering interval in seconds (adjust to control flicker speed)
    public float flickerInterval = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_WEBGL
        this.gameObject.SetActive(false);
        #endif
        // Initialize flickering by setting all lights to "off" (material[0]) initially
        int numOfChildren = transform.childCount;
        for (int i = 0; i < numOfChildren; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            rend = child.GetComponent<Renderer>();
            rend.enabled = true;
            rend.sharedMaterial = material[0];
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Only perform flickering if the mouse is not over the object
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
        if(panelSelectLevel.gameObject.activeSelf == false && panelOptions.gameObject.activeSelf == false) 
        {
            OptionsManager.StopVibration();
            Application.Quit();
            Debug.Log("Application Quit");
        }
    }

    private void OnMouseEnter()
    {
        if (panelSelectLevel.gameObject.activeSelf == false && panelOptions.gameObject.activeSelf == false)
        {
            // Set isMouseOver to true to stop flickering
            isMouseOver = true;

            // Turn all lights on
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
            // Set isMouseOver to false to resume flickering
            isMouseOver = false;
        }
    }
}
