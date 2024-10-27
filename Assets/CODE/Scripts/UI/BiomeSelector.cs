using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class BiomeSelector : MonoBehaviour
{
    public TextMeshProUGUI biomeNameText;  // Text element to display biome name
    public Button previousBiomeButton;     // Button for previous biome
    public Button nextBiomeButton;         // Button for next biome

    public int currentBiomeIndex = 1; // The currently selected biome number (starts at 1)

    // Dictionary to hold biome numbers as keys and their corresponding names
    private Dictionary<int, string> biomes = new Dictionary<int, string>
    {
        { 1, "Forest" },
        { 2, "Desert" },
        { 3, "Ice Land" },
        // Add more biomes as needed
    };

    private void Start()
    {
        UpdateBiomeUI(); // Initialize the UI with the first biome

        previousBiomeButton.onClick.AddListener(SelectPreviousBiome);
        nextBiomeButton.onClick.AddListener(SelectNextBiome);
    }

    private void UpdateBiomeUI()
    {
        biomeNameText.text = biomes[currentBiomeIndex];

        previousBiomeButton.interactable = currentBiomeIndex > 1;
        nextBiomeButton.interactable = currentBiomeIndex < biomes.Count;

        // Notify LevelSelectManager to update the level buttons
        if (LevelSelectManager.Instance != null)
        {
            LevelSelectManager.Instance.UpdateLevelButtonsForBiome(currentBiomeIndex);
        }
    }

    public void SelectPreviousBiome()
    {
        if (currentBiomeIndex > 1)
        {
            currentBiomeIndex--;
            UpdateBiomeUI();
        }
    }

    public void SelectNextBiome()
    {
        if (currentBiomeIndex < biomes.Count)
        {
            currentBiomeIndex++;
            UpdateBiomeUI();
        }
    }
}
