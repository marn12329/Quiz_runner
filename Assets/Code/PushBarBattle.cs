using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PushBarBattle : MonoBehaviour
{
    [Header("Progress Bars (แทน powerCircles เดิม)")]
    public Image[] powerBars;
    public RectTransform[] handles;
    public Image[] handleImages;
    public float handleOffset = 10f;

    [Header("UI")]
    public TextMeshProUGUI resultText;
    public float pushAmount = 0.05f;

    [Header("Player Buttons (ตั้งเองใน Inspector)")]
    public Button[] playerButtons;

    [Header("Keyboard Keys for Each Player (ตั้งปุ่มเองได้)")]
    public KeyCode[] playerKeys;

    private float[] powerValues;
    private bool gameRunning = true;
    private bool inputEnabled = false;

    public Action<int> OnGameEnd;

    void Start()
    {
        int count = Mathf.Max(4, powerBars.Length);
        powerValues = new float[count];
        ResetBattle();

        // ผูกปุ่ม UI ให้ผู้เล่นแต่ละคน
        if (playerButtons != null)
        {
            for (int i = 0; i < playerButtons.Length; i++)
            {
                int index = i;
                if (playerButtons[i] != null)
                    playerButtons[i].onClick.AddListener(() => OnPlayerButtonPress(index));
            }
        }
    }

    void Update()
    {
        if (!gameRunning) return;

        UpdateBars();
        UpdateHandles();

        // 🔥 เช็คปุ่มคีย์บอร์ด
        CheckKeyboardInput();
    }

    // ----------------------------
    //    Keyboard Input System
    // ----------------------------
    void CheckKeyboardInput()
    {
        if (!inputEnabled || playerKeys == null) return;

        for (int i = 0; i < playerKeys.Length; i++)
        {
            if (i < PlayerData.freezeTurns.Length && PlayerData.freezeTurns[i] > 0)
                continue;

            if (i < PlayerData.skipNextTurn.Length && PlayerData.skipNextTurn[i])
                continue;

            if (Input.GetKeyDown(playerKeys[i]))
            {
                OnPlayerButtonPress(i);
            }
        }
    }

    // ----------------------------
    //         กดปุ่ม
    // ----------------------------
    void OnPlayerButtonPress(int player)
    {
        if (!gameRunning || !inputEnabled) return;

        if (player >= 0 && player < PlayerData.freezeTurns.Length &&
            PlayerData.freezeTurns[player] > 0)
            return;

        PushPower(player);
    }

    void PushPower(int player)
    {
        if (player < 0 || player >= powerValues.Length) return;

        float actualPush = pushAmount;

        if (player < PlayerData.slowTurns.Length && PlayerData.slowTurns[player] > 0)
            actualPush = Mathf.Max(0.01f, pushAmount * 0.2f);

        powerValues[player] += actualPush;
        powerValues[player] = Mathf.Clamp01(powerValues[player]);

        if (powerValues[player] >= 1f)
        {
            gameRunning = false;
            if (resultText != null)
                resultText.text = $"{PlayerData.GetPlayerName(player)} ชนะ!";

            OnGameEnd?.Invoke(player);
        }
    }

    // ----------------------------
    //       UI / Handle Update
    // ----------------------------
    void UpdateBars()
    {
        for (int i = 0; i < powerBars.Length; i++)
        {
            if (powerBars[i] != null)
                powerBars[i].fillAmount = powerValues[i];
        }
    }

    void UpdateHandles()
    {
        for (int i = 0; i < powerBars.Length; i++)
        {
            if (handles == null || i >= handles.Length || handles[i] == null || powerBars[i] == null)
                continue;

            RectTransform barRect = powerBars[i].rectTransform;
            float barWidth = barRect.rect.width;

            float handleX = -barWidth / 2f + (powerBars[i].fillAmount * barWidth);
            handleX += handleOffset;

            Vector2 newPos = handles[i].anchoredPosition;
            newPos.x = handleX;
            handles[i].anchoredPosition = newPos;
        }
    }

    // ----------------------------
    //         Reset
    // ----------------------------
    public void ResetBattle()
    {
        for (int i = 0; i < powerBars.Length; i++)
        {
            if (i >= powerValues.Length) continue;

            powerValues[i] = 0;

            if (powerBars[i] != null)
                powerBars[i].fillAmount = 0f;

            if (handles != null &&
                i < handles.Length &&
                handles[i] != null &&
                powerBars[i] != null)
            {
                Vector2 newPos = handles[i].anchoredPosition;
                newPos.x = -powerBars[i].rectTransform.rect.width / 2f;
                handles[i].anchoredPosition = newPos;
            }
        }

        TryAssignHandleSpritesFromPrefs();

        if (resultText != null)
            resultText.text = "";

        gameRunning = true;
        SetInputEnabled(false);
    }

    // ----------------------------
    //   Assign Character Sprites
    // ----------------------------
    private void TryAssignHandleSpritesFromPrefs()
    {
        if (handleImages == null) return;

        for (int i = 0; i < handleImages.Length; i++)
        {
            if (handleImages[i] == null) continue;

            string charName = PlayerData.GetPlayerCharacter(i);

            if (string.IsNullOrEmpty(charName))
            {
                charName = PlayerPrefs.GetString($"Player{i}_Character", "");
                if (string.IsNullOrEmpty(charName))
                    charName = PlayerPrefs.GetString($"Player{i + 1}_Character", "");
            }

            if (!string.IsNullOrEmpty(charName))
            {
                Sprite s = CharacterLookup.GetSprite(charName);
                if (s == null)
                    s = CharacterLookup.TryLoadFromResources(charName);

                if (s != null)
                {
                    handleImages[i].sprite = s;
                    handleImages[i].preserveAspect = true;
                }
            }
        }
    }

    // ----------------------------
    //     Enable / Disable Input
    // ----------------------------
    public void SetInputEnabled(bool enabled)
    {
        inputEnabled = enabled;

        if (playerButtons == null) return;

        for (int i = 0; i < playerButtons.Length; i++)
        {
            if (playerButtons[i] == null) continue;

            bool disable = false;

            if (i < PlayerData.freezeTurns.Length && PlayerData.freezeTurns[i] > 0)
                disable = true;
            if (i < PlayerData.skipNextTurn.Length && PlayerData.skipNextTurn[i])
                disable = true;

            playerButtons[i].interactable = enabled && !disable;
        }
    }

    // ----------------------------
    //       Force End Game
    // ----------------------------
    public void ForceEndByTime()
    {
        int winner = -1;
        float maxValue = -1f;

        for (int i = 0; i < powerValues.Length; i++)
        {
            if (powerValues[i] > maxValue)
            {
                maxValue = powerValues[i];
                winner = i;
            }
        }

        gameRunning = false;

        if (winner != -1)
        {
            if (resultText != null)
                resultText.text = $"{PlayerData.GetPlayerName(winner)} ชนะ!";

            OnGameEnd?.Invoke(winner);
        }
    }
}
