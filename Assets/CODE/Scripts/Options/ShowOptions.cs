using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        optionsPanel.SetActive(true);
        mainUI.SetActive(false);
    }

    public void backButton()
    {
        optionsPanel.SetActive(false);
        mainUI.SetActive(true);       
    }
}
