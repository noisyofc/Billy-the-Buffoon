using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene management

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance = null; // Singleton instance
    public AudioSource backgroundMusic; // Reference to the AudioSource component
    public string menuSceneName = "Menu"; // The name of the menu scene where the music should stop

    public static float musicTime = 0f;

    void Awake()
    {
        // Ensure that there is only one instance of this object (Singleton pattern)
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject); // Make this GameObject persistent across scenes
        }
        else if (instance != this)
        {
            Destroy(gameObject); // Destroy duplicates
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        // Start playing background music if it's not already playing
        if (backgroundMusic != null && !backgroundMusic.isPlaying)
        {
            backgroundMusic.Play();
        }

        // Subscribe to the scene change event
        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        backgroundMusic.time = musicTime;
        backgroundMusic.Play();
    }

    public void SaveMusicTime()
    {
        musicTime = backgroundMusic.time;
    }

    // This method is called whenever the active scene changes
    private void OnSceneChanged(Scene previousScene, Scene newScene)
    {
        // Check if the new scene is the menu scene
        //if (newScene.name == menuSceneName)
        //{
        //    StopMusic();
        //}
        //else
        //{
        //    StartCoroutine(PlayMusic());
        //}
    }

    public void SetVolume(float volume)
    {
        backgroundMusic.volume = volume; // Adjust volume of the music
    }

    public void StopMusic()
    {
        if (backgroundMusic.isPlaying)
        {
            backgroundMusic.Stop(); // Stop the music if it's playing
        }
    }

    IEnumerator PlayMusic()
    {
        yield return new WaitForEndOfFrame();

        if (!backgroundMusic.isPlaying)
        {
            backgroundMusic.Play(); // Play the music if it's not already playing
        }
    }
}