using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MockMenuExit : MonoBehaviour
{
    Renderer rend;
    public Material[] material;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
        rend.enabled = true;
        rend.sharedMaterial = material[0];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseUpAsButton()
    {
        Application.Quit();
    }

    private void OnMouseEnter()
    {
        rend.sharedMaterial = material[1];
    }

    private void OnMouseExit()
    {
        rend.sharedMaterial = material[0];
    }

}
