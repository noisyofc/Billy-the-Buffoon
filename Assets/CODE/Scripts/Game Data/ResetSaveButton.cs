using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ResetSaveButton : MonoBehaviour
{
    public Button resetButton; // Reference to the reset button

    private void Start()
    {
        // Ensure the reset button triggers the reset functionality when clicked
        resetButton.onClick.AddListener(ResetSaveData);
    }

    private void ResetSaveData()
    {
        // Path to the save file
        string filePath = Application.persistentDataPath + "/saveData.json";

        // Check if the save file exists
        if (File.Exists(filePath))
        {
            // Delete the save file
            File.Delete(filePath);
            Debug.Log("Save data has been reset to initial state.");

            // Call method to lock all levels after deleting save data
            if (LevelSelectManager.Instance != null)
            {
                LevelSelectManager.Instance.LockAllLevels();
            }
        }
        else
        {
            Debug.LogWarning("No save data found to reset.");
        }
    }
}
