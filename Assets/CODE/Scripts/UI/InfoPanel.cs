using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoPanel : MonoBehaviour
{
    public static bool panelClosed = false;
    // Start is called before the first frame update
    void Start()
    {
        if (panelClosed == true)
        {
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void closePanel()
    {
        gameObject.SetActive(false);
        panelClosed = true;
    }
}
