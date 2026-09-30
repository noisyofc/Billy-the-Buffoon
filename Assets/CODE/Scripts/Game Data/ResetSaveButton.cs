using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ResetSaveButton : MonoBehaviour
{
    public Button resetButton;

    private void Start()
    {
        if (resetButton != null)
            resetButton.onClick.AddListener(ResetSaveData);
    }

    private void ResetSaveData()
    {
        SaveSystem.DeleteSave();

        if (LevelSelectManager.Instance != null)
            LevelSelectManager.Instance.LockAllLevels();
    }
}
