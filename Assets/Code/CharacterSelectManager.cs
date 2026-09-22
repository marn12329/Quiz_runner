using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class CharacterSelectManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Button[] characterButtons;          // ปุ่มเลือกตัวละคร
    public TextMeshProUGUI statusText;         // ข้อความสถานะ

    private List<int> playerOrder = new List<int>();
    private int currentPlayerIndex = 0;
    private Dictionary<int, string> selectedCharacters = new Dictionary<int, string>();
    private string[] tempPlayerNames;

    void Start()
    {
        LoadOrResetPlayerOrder();
        ValidateUI();
        AssignButtonEvents();

        tempPlayerNames = new string[4]; // รองรับ 4 ผู้เล่น
        LoadPlayerNames(); // โหลดชื่อจาก PlayerData (subject-aware)
        UpdateStatusText();
    }

    private void LoadOrResetPlayerOrder()
    {
        string orderString = PlayerPrefs.GetString("PlayerOrder", "");
        playerOrder.Clear();

        if (!string.IsNullOrEmpty(orderString))
        {
            foreach (string s in orderString.Split(','))
            {
                if (int.TryParse(s, out int p))
                    playerOrder.Add(p);
            }
        }

        if (playerOrder.Count != 4)
        {
            playerOrder.Clear();
            for (int i = 0; i < 4; i++)
                playerOrder.Add(i);

            PlayerPrefs.SetString("PlayerOrder", string.Join(",", playerOrder));
            PlayerPrefs.Save();
        }

        Debug.Log("Loaded PlayerOrder: " + string.Join(",", playerOrder));
    }

    private void ValidateUI()
    {
        if (characterButtons == null || characterButtons.Length == 0)
            Debug.LogError("Character buttons are not assigned in the Inspector!");

        if (statusText == null)
            Debug.LogError("Status Text is not assigned in the Inspector!");
    }

    private void AssignButtonEvents()
    {
        foreach (Button btn in characterButtons)
        {
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                string prefabName = btn.name;
                Button tempBtn = btn;
                string tempName = prefabName;

                tempBtn.onClick.AddListener(() => OnCharacterButtonClick(tempName, tempBtn));
            }
        }
    }

    private void OnCharacterButtonClick(string prefabName, Button btn)
    {
        if (currentPlayerIndex < playerOrder.Count)
        {
            int playerID = playerOrder[currentPlayerIndex];
            Debug.Log($"Player {playerID + 1} picked {prefabName}");

            selectedCharacters[playerID] = prefabName;

            // เก็บชั่วคราวชื่อ (จาก PlayerData)
            tempPlayerNames[playerID] = PlayerData.GetPlayerName(playerID);

            btn.interactable = false;
            currentPlayerIndex++;

            if (currentPlayerIndex >= playerOrder.Count)
            {
                if (selectedCharacters.Count == playerOrder.Count)
                {
                    SaveSelectedCharacters();
                    Debug.Log("All players picked. Loading MiniGameScene...");
                    if (SceneFlowManager.Instance != null)
                        SceneFlowManager.Instance.GoToMiniGame();
                    else
                        Debug.LogError("SceneFlowManager.Instance is null when trying to GoToMiniGame");
                }
            }
            else
            {
                UpdateStatusText();
            }
        }
    }

    private void UpdateStatusText()
    {
        if (currentPlayerIndex < playerOrder.Count)
        {
            int currentPlayer = playerOrder[currentPlayerIndex];
            string playerName = PlayerData.GetPlayerName(currentPlayer);
            statusText.text = $"{playerName}, pick your character!";
        }
        else
        {
            statusText.text = "All players have picked!";
        }
    }

    private void SaveSelectedCharacters()
    {
        foreach (var kvp in selectedCharacters)
        {
            // บันทึกผ่าน PlayerData (save Global + subject-specific)
            PlayerData.SetPlayerCharacter(kvp.Key, kvp.Value);

            // บันทึกชื่อ (ถ้าต้องการ update) -> บันทึกผ่าน PlayerData (Global + subject)
            if (!string.IsNullOrEmpty(tempPlayerNames[kvp.Key]))
                PlayerData.SetPlayerName(kvp.Key, tempPlayerNames[kvp.Key]);
        }

        PlayerPrefs.Save();

        Debug.Log("Saved selected characters: " + string.Join(", ", selectedCharacters));
    }

    private void LoadPlayerNames()
    {
        for (int i = 0; i < tempPlayerNames.Length; i++)
        {
            tempPlayerNames[i] = PlayerData.GetPlayerName(i);
        }
        Debug.Log("Loaded player names: " + string.Join(", ", tempPlayerNames));
    }
}
