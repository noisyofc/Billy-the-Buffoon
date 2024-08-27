using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MockMenuTestLevel : MonoBehaviour
{

    public GameObject panelSelectLevel, panelOptions;
    Renderer rend;
    public Material[] material;
    // Start is called before the first frame update
    void Start()
    {
        //rend = GetComponent<Renderer>();
        //rend.enabled = true;
        //rend.sharedMaterial = material[0];

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
        if (panelSelectLevel.gameObject.activeSelf == true)
        {
            int numOfChildren = transform.childCount;
            for (int i = 0; i < numOfChildren; i++)
            {
                GameObject child = transform.GetChild(i).gameObject;
                rend = child.GetComponent<Renderer>();
                rend.enabled = true;
                rend.sharedMaterial = material[0];
            }
        }
    }

    private void OnMouseUpAsButton()
    {
        if (panelSelectLevel.gameObject.activeSelf == false && panelOptions.gameObject.activeSelf == false)
        {
            //SceneManager.LoadScene("Scenes/Level 0 - TEST_LEVEL");
            //Time.timeScale = 1;
            panelSelectLevel.gameObject.SetActive(true);
        }
    }

    private void OnMouseEnter()
    {
        //rend.sharedMaterial = material[1];
        if (panelSelectLevel.gameObject.activeSelf == false && panelOptions.gameObject.activeSelf == false)
        {
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
            //rend.sharedMaterial = material[0];

            int numOfChildren = transform.childCount;
            for (int i = 0; i < numOfChildren; i++)
            {
                GameObject child = transform.GetChild(i).gameObject;
                rend = child.GetComponent<Renderer>();
                rend.enabled = true;
                rend.sharedMaterial = material[0];
            }
        }

    }
}
