using UnityEngine;
using UnityEngine.UI;
using TMPro; // เพิ่มบรรทัดนี้
using System;

public class PushCircleBattle : MonoBehaviour
{
    public Image[] powerCircles;
    public TextMeshProUGUI resultText; // ใช้ TMP แทน Text
    public float pushAmount = 0.05f;

    private float[] powerValues;
    private bool gameRunning = true;

    public Action<int> OnGameEnd;

    void Start()
    {
        powerValues = new float[powerCircles.Length];
        ResetBattle();
    }

    void Update()
    {
        if (!gameRunning) return;

        foreach (Touch t in Input.touches)
        {
            if (t.phase == TouchPhase.Began)
            {
                int player = GetTouchPlayer(t.position);
                if (player != -1) PushPower(player);
            }
        }

        UpdateCircles();
    }

    void PushPower(int player)
    {
        powerValues[player] += pushAmount;
        powerValues[player] = Mathf.Clamp01(powerValues[player]);

        if (powerValues[player] >= 1f)
        {
            gameRunning = false;
            if (resultText != null) resultText.text = "ผู้เล่น " + (player + 1) + " ชนะ!";
            OnGameEnd?.Invoke(player);
        }
    }

    void UpdateCircles()
    {
        for (int i = 0; i < powerCircles.Length; i++)
            powerCircles[i].fillAmount = powerValues[i];
    }

    int GetTouchPlayer(Vector2 pos)
    {
        float w = Screen.width;
        float h = Screen.height;

        if (pos.x < w / 2 && pos.y > h / 2) return 0;
        if (pos.x > w / 2 && pos.y > h / 2) return 1;
        if (pos.x < w / 2 && pos.y < h / 2) return 2;
        if (pos.x > w / 2 && pos.y < h / 2) return 3;
        return -1;
    }

    public void ResetBattle()
    {
        for (int i = 0; i < powerCircles.Length; i++)
        {
            powerValues[i] = 0;
            if (powerCircles[i] != null) powerCircles[i].fillAmount = 0f;
        }
        if (resultText != null) resultText.text = "";
        gameRunning = true;
    }

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
            if (resultText != null) resultText.text = "ผู้เล่น " + (winner + 1) + " ชนะ!";
            OnGameEnd?.Invoke(winner);
        }
    }
}
