using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EndScreen : MonoBehaviour
{

    public TextMeshProUGUI starsEnd;
    public TextMeshProUGUI timeEnd;

    public int stars = 0;
    public float time = 0f;

    public Image endScreen;
    public GameObject canvas;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        starsEnd.text = stars.ToString();

        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        timeEnd.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Time.timeScale = 0;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            canvas.SetActive(false);
            endScreen.gameObject.SetActive(true);

            stars = CountStars.instance.stars;
            time = Timer.instance.timeElapsed;

        }
    }
}
