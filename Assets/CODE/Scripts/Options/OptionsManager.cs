using UnityEngine;

public class OptionsManager : MonoBehaviour
{
    public GameObject generalPanel;
    public GameObject videoPanel;
    public GameObject audioPanel;
    public GameObject controlsPanel;

    public void ShowGeneral()
    {
        generalPanel.SetActive(true);
        videoPanel.SetActive(false);
        audioPanel.SetActive(false);
        controlsPanel.SetActive(false);
    }

    public void ShowVideo()
    {
        generalPanel.SetActive(false);
        videoPanel.SetActive(true);
        audioPanel.SetActive(false);
        controlsPanel.SetActive(false);
    }

    public void ShowAudio()
    {
        generalPanel.SetActive(false);
        videoPanel.SetActive(false);
        audioPanel.SetActive(true);
        controlsPanel.SetActive(false);
    }
    
    public void ShowControls()
    {
        controlsPanel.SetActive(true);
        generalPanel.SetActive(false);
        videoPanel.SetActive(false);
        audioPanel.SetActive(false);
    }
}
