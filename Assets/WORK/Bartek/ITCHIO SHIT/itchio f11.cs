using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class itchiof11 : MonoBehaviour
{
    public static itchiof11 Instance;
    public GameObject levels, settings, start, quit;
    private bool pressedF11 = false;

    // Start is called before the first frame update

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate
        }
    }

    void Start()
    {
#if UNITY_WEBGL
        if (pressedF11 == false)
        {
            levels.GetComponent<BoxCollider>().enabled = false;
            settings.GetComponent<BoxCollider>().enabled = false;
            start.GetComponent<BoxCollider>().enabled = false;
            quit.GetComponent<BoxCollider>().enabled = false;
        }
        #endif
#if !UNITY_WEBGL
        gameObject.SetActive(false);

#endif
        }

    // Update is called once per frame
    void Update()
    {
#if UNITY_WEBGL
        if (pressedF11 == false)
        {
            gameObject.SetActive(true);
        }
        if (pressedF11 == true)
        {
            gameObject.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.F11) && gameObject.activeInHierarchy == true && pressedF11 == false)
        {
            gameObject.SetActive(false);
            levels.GetComponent<BoxCollider>().enabled = true;
            settings.GetComponent<BoxCollider>().enabled = true;
            start.GetComponent<BoxCollider>().enabled = true;
            quit.GetComponent<BoxCollider>().enabled = true;
            pressedF11 = true;
            // Add your logic here
        }

#endif

    }
}
