using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Banner : MonoBehaviour
{
    public GameObject credits, levels, settings, start, quit, whatsNext;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Credits()
    {
        credits.gameObject.SetActive(true);
        gameObject.SetActive(false);
        levels.GetComponent<BoxCollider>().enabled = false;
        settings.GetComponent<BoxCollider>().enabled = false;
        start.GetComponent<BoxCollider>().enabled = false;
        quit.GetComponent<BoxCollider>().enabled = false;
    }

    public void ExitCredits()
    {
        credits.gameObject.SetActive(false);
        gameObject.SetActive(true);
        levels.GetComponent<BoxCollider>().enabled = true;
        settings.GetComponent<BoxCollider>().enabled = true;
        start.GetComponent<BoxCollider>().enabled = true;
        quit.GetComponent<BoxCollider>().enabled = true;
    }

    public void whatsNextEnter()
    {
        whatsNext.gameObject.SetActive(true);
        gameObject.SetActive(false);
        levels.GetComponent<BoxCollider>().enabled = false;
        settings.GetComponent<BoxCollider>().enabled = false;
        start.GetComponent<BoxCollider>().enabled = false;
        quit.GetComponent<BoxCollider>().enabled = false;
    }

    public void whatsNextExit()
    {
        whatsNext.gameObject.SetActive(false);
        gameObject.SetActive(true);
        levels.GetComponent<BoxCollider>().enabled = true;
        settings.GetComponent<BoxCollider>().enabled = true;
        start.GetComponent<BoxCollider>().enabled = true;
        quit.GetComponent<BoxCollider>().enabled = true;
    }

    public void LinkFeedback()
    {
        Application.OpenURL("https://forms.gle/UWMg4uSt156JQEJq9");
    }
    
    public void LinkBugReport()
    {
        Application.OpenURL("https://forms.gle/mTKvpieibTBNdz216");
    }

}
