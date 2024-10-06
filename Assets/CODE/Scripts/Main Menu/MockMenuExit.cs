using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MockMenuExit : MonoBehaviour
{
    Renderer rend;
    public Material[] material;
    public GameObject panelSelectLevel, panelOptions;

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
        
    }

    private void OnMouseUpAsButton()
    {
        if(panelSelectLevel.gameObject.activeSelf == false && panelOptions.gameObject.activeSelf == false) {
            Application.Quit();
            Debug.Log("TEST");
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
        //rend.sharedMaterial = material[0];
        if (panelSelectLevel.gameObject.activeSelf == false && panelOptions.gameObject.activeSelf == false)
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

}
