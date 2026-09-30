using UnityEngine;

public class itchiof11 : MonoBehaviour
{
    public static itchiof11 Instance;
    public GameObject levels, settings, start, quit;

#if UNITY_WEBGL
    private bool pressedF11;
#endif

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
#if UNITY_WEBGL
        if (!pressedF11)
        {
            levels.GetComponent<BoxCollider>().enabled = false;
            settings.GetComponent<BoxCollider>().enabled = false;
            start.GetComponent<BoxCollider>().enabled = false;
            quit.GetComponent<BoxCollider>().enabled = false;
        }
#else
        gameObject.SetActive(false);
#endif
    }

    void Update()
    {
#if UNITY_WEBGL
        if (!pressedF11)
            gameObject.SetActive(true);
        else
            gameObject.SetActive(false);

        if (Input.GetKeyDown(KeyCode.F11) && gameObject.activeInHierarchy && !pressedF11)
        {
            gameObject.SetActive(false);
            levels.GetComponent<BoxCollider>().enabled = true;
            settings.GetComponent<BoxCollider>().enabled = true;
            start.GetComponent<BoxCollider>().enabled = true;
            quit.GetComponent<BoxCollider>().enabled = true;
            pressedF11 = true;
        }
#endif
    }
}
