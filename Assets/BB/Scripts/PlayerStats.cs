using System;
using Firebase.Firestore;

[FirestoreData]  // Optional but recommended
[Serializable]
public class PlayerStats
{
    [FirestoreProperty]
    public string playerName { get; set; }

    [FirestoreProperty]
    public int highestScore { get; set; }

    [FirestoreProperty]
    public int longestCombo { get; set; }

    [FirestoreProperty]
    public int totalPlantsBloomed { get; set; }

    // Parameterless constructor required by Firestore
    public PlayerStats() { }

    public PlayerStats(string name)
    {
        playerName = name;
        highestScore = 0;
        longestCombo = 0;
        totalPlantsBloomed = 0;
    }
}

