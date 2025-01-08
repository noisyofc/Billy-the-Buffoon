using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowOptions : MonoBehaviour
{
    public GameObject mainUI, optionsPanel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void showPanelOptions()
    {
        optionsPanel.GetComponent<Canvas>().enabled = true;
        mainUI.SetActive(false);
    }

    public void backButton()
    {
        optionsPanel.GetComponent<Canvas>().enabled = false;
        mainUI.SetActive(true);  
    }
}
