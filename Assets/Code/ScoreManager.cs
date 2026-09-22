using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int[] playerScores = new int[4];
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI[] rankTexts;
    public GameObject scoreboardPanel;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void AddScore(int playerID, int amount)
    {
        if (playerID >= 0 && playerID < playerScores.Length)
            playerScores[playerID] += amount;
    }

    public void ResetScores()
    {
        for (int i = 0; i < playerScores.Length; i++)
            playerScores[i] = 0;
    }

    public int GetScore(int playerID)
    {
        return (playerID >= 0 && playerID < playerScores.Length)
            ? playerScores[playerID]
            : 0;
    }

    public void UpdateScoreboard()
    {
        if (scoreboardPanel != null)
            scoreboardPanel.SetActive(true);

        if (titleText != null)
            titleText.text = "🏁 ผลการแข่งขัน 🏁";

        string[] playerNames = PlayerData.playerNames;

        int[] sortedIndex = { 0, 1, 2, 3 };
        System.Array.Sort(sortedIndex, (a, b) => playerScores[b].CompareTo(playerScores[a]));

        for (int i = 0; i < rankTexts.Length; i++)
        {
            int idx = sortedIndex[i];
            if (rankTexts[i] != null)
                rankTexts[i].text = $"{i + 1}. {PlayerData.GetPlayerName(idx)} - {playerScores[idx]} คะแนน";
        }
    }
}
