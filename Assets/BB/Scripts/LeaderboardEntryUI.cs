using UnityEngine;
using TMPro;

public class LeaderboardEntryUI : MonoBehaviour
{
    [Header("UI Text References")]
    public TextMeshProUGUI rankText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI scoreText;

    public void SetData(int rank, string playerName, string score)
    {
        if (rankText != null) rankText.text = $"{rank}.";
        if (nameText != null) nameText.text = playerName;
        if (scoreText != null) scoreText.text = score;
    }
}
