using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class BiomeSelector : MonoBehaviour
{
    //public TextMeshProUGUI biomeNumberText; // Text element to display biome number
    public TextMeshProUGUI biomeNameText;   // Text element to display biome name
    public Button previousBiomeButton; // Button for previous biome
    public Button nextBiomeButton;     // Button for next biome

    public int currentBiomeIndex = 1; // The currently selected biome number (starts at 1)

    // Dictionary to hold biome numbers as keys and their corresponding names
    private Dictionary<int, string> biomes = new Dictionary<int, string>
    {
        { 1, "Forest" },
        { 2, "Desert" },
        { 3, "Ice Land" },
    };

    private void Start()
    {
        UpdateBiomeUI(); // Initialize the UI with the first biome

        // Assign button onClick listeners
        previousBiomeButton.onClick.AddListener(SelectPreviousBiome);
        nextBiomeButton.onClick.AddListener(SelectNextBiome);
    }

    private void UpdateBiomeUI()
    {
        // Update the displayed biome number and name
        //biomeNumberText.text = string.Format("{0}",currentBiomeIndex);
        biomeNameText.text = biomes[currentBiomeIndex];

        // Enable/Disable buttons based on the biome index
        previousBiomeButton.interactable = currentBiomeIndex > 1;
        nextBiomeButton.interactable = currentBiomeIndex < biomes.Count;
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
