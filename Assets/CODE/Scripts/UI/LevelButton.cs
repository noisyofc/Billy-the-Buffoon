using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    public int biomeNumber;        // The biome number this level belongs to
    public int levelNumber;        // The level number within the biome
    public bool isUnlocked;        // Indicates if the level is unlocked

    private Button button;
    private Image image;

    public Sprite defaultSprite;
    public Sprite selectedSprite;
    public Sprite disabledSprite;

    // Reference to the background GameObject
    public GameObject background;

    private void Awake()
    {
        // Initialize button and image components in Awake to ensure they are available
        button = GetComponent<Button>();
        image = GetComponent<Image>();

        if (button == null)
        {
            Debug.LogError("Button component not found on LevelButton object.");
            return;
        }
        if (image == null)
        {
            Debug.LogError("Image component not found on LevelButton object.");
            return;
        }

        // Set background inactive by default
        if (background != null)
        {
            background.SetActive(false);
        }
    }

    private void Start()
    {
        // Initialize the button state based on the unlock status
        UpdateButtonState();
    }

    // Method to update the button's appearance and interaction based on isUnlocked
    public void UpdateButtonState()
    {
        if (button == null || image == null)
        {
            Debug.LogError("Button or Image component is missing.");
            return;
        }

        // Set button interactable state and sprite based on isUnlocked
        button.interactable = isUnlocked;
        image.sprite = isUnlocked ? defaultSprite : disabledSprite;

        // Update button click listeners safely
        ResetButtonListeners();
        if (isUnlocked)
        {
            button.onClick.AddListener(OnButtonClick);
        }
    }

    // Reset button listeners to prevent duplicate listeners from being added
    private void ResetButtonListeners()
    {
        if (button != null)
        {
            button.onClick.RemoveAllListeners();  // Clears any previously added listeners
        }
    }

    private void OnButtonClick()
    {
        if (LevelSelectManager.Instance != null)
        {
            LevelSelectManager.Instance.SelectLevel(this);
        }
    }

    public void SetSelected(bool selected)
    {
        if (image != null)
        {
            image.sprite = selected ? selectedSprite : defaultSprite;
        }

        // Activate or deactivate background GameObject
        if (background != null)
        {
            background.SetActive(selected);
        }
    }
}
