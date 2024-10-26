using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    public int levelNumber;
    public bool isUnlocked;
    private Button button;
    private Image image;

    public Sprite defaultSprite;
    public Sprite selectedSprite;
    public Sprite disabledSprite;

    // Reference to the background GameObject
    public GameObject background;

    private void Start()
    {
        button = GetComponent<Button>();
        image = GetComponent<Image>();

        // Set background inactive by default
        if (background != null)
        {
            background.SetActive(false);
        }

        // Disable button if the level is locked
        if (!isUnlocked)
        {
            button.interactable = false;
            image.sprite = disabledSprite;
        }
        else
        {
            button.onClick.AddListener(OnButtonClick);
            image.sprite = defaultSprite;
        }
    }

    private void OnButtonClick()
    {
        LevelSelectManager.Instance.SelectLevel(this);
    }

    public void SetSelected(bool selected)
    {
        image.sprite = selected ? selectedSprite : defaultSprite;

        // Activate or deactivate background GameObject
        if (background != null)
        {
            background.SetActive(selected);
        }
    }
}
