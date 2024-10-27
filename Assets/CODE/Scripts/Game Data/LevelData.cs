using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelData
{
    public string biomeNumber;
    public string levelNumber;
    public string isUnlocked;         // "true" or "false"
    public string bestTime;           // Best time as a string
    public string bestBalloons;       // Best balloons collected as a string
    public string grade;              // Grade as a string (e.g., "C+")
}
