using UnityEngine;
using TMPro;
using System.Collections;
using System;

public class NewMiniGame : MonoBehaviour
{
    [Header("MiniGame Logic")]
    public PushBarBattle pushBarBattle;

    [Header("UI Elements")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI questionText;

    [Header("Player Name Display")]
    public TextMeshProUGUI[] playerNameTexts;

    [Header("Settings")]
    public float showWinnerDelay = 3f;
    public float miniGameDuration = 10f;
    [TextArea] public string[] questions;
    public bool showQuestionBox = true;

    [Header("Floating Answer Buttons (Optional)")]
    public bool useFloatingButtons = true;
    public bool stopFloatingWhenTimeUp = true;

    public bool IsFinished { get; private set; }
    public int WinnerIndex { get; private set; } = -1;

    public Action<int> OnMiniGameFinished;

    private bool isRunning = false;

    // lastWinner passed for display before start
    public void StartMiniGame(int lastWinner = -1)
    {
        StopAllCoroutines();
        IsFinished = false;
        WinnerIndex = -1;
        isRunning = true;

        UpdatePlayerNameDisplay();

        if (showQuestionBox)
            ShowRandomQuestion();
        else if (questionText != null)
            questionText.gameObject.SetActive(false);

        if (useFloatingButtons)
            SetFloatingButtonsActive(true);

        if (pushBarBattle != null)
        {
            pushBarBattle.ResetBattle();
            pushBarBattle.OnGameEnd = OnPushBattleEnded;
            pushBarBattle.SetInputEnabled(false); // lock until showWinnerBeforeStart ends
        }

        StartCoroutine(ShowWinnerBeforeStart(lastWinner));
    }

    private void UpdatePlayerNameDisplay()
    {
        if (playerNameTexts == null) return;
        for (int i = 0; i < playerNameTexts.Length; i++)
        {
            if (playerNameTexts[i] != null)
                playerNameTexts[i].text = PlayerData.GetPlayerName(i);
        }
    }

    private void ShowRandomQuestion()
    {
        if (questionText == null || questions == null || questions.Length == 0) return;

        questionText.gameObject.SetActive(true);
        string randomQuestion = questions[UnityEngine.Random.Range(0, questions.Length)];
        questionText.text = randomQuestion;
    }

    IEnumerator ShowWinnerBeforeStart(int lastWinner)
    {
        if (resultText != null)
        {
            if (lastWinner != -1)
                resultText.text = $"รอบก่อน {PlayerData.GetPlayerName(lastWinner)} เป็นผู้ชนะ!";
            else
                resultText.text = "เตรียมตัวให้พร้อม!";
        }

        yield return new WaitForSeconds(showWinnerDelay);

        if (resultText != null)
            resultText.text = "เกมเริ่มแล้ว!";

        if (pushBarBattle != null)
            pushBarBattle.SetInputEnabled(true);

        StartCoroutine(MiniGameTimer());
    }

    IEnumerator MiniGameTimer()
    {
        float timer = miniGameDuration;

        while (timer > 0f && WinnerIndex == -1)
        {
            timer -= Time.deltaTime;

            if (timerText != null)
                timerText.text = $"เวลา: {Mathf.Ceil(timer)} วิ";

            yield return null;
        }

        if (WinnerIndex == -1 && pushBarBattle != null)
        {
            if (resultText != null)
                resultText.text = "หมดเวลา!";

            if (useFloatingButtons && stopFloatingWhenTimeUp)
                SetFloatingButtonsActive(false);

            pushBarBattle.ForceEndByTime();
            yield return new WaitForSeconds(1.5f);
        }

        while (WinnerIndex == -1)
            yield return null;
    }

    private void OnPushBattleEnded(int winner)
    {
        if (WinnerIndex != -1) return;

        WinnerIndex = winner;

        if (resultText != null)
        {
            if (winner >= 0)
                resultText.text = $"ผู้ชนะคือ {PlayerData.GetPlayerName(winner)}!";
            else
                resultText.text = "ไม่มีผู้ชนะ!";
        }

        if (pushBarBattle != null)
            pushBarBattle.SetInputEnabled(false);

        if (useFloatingButtons)
            SetFloatingButtonsActive(false);

        StartCoroutine(DelayFinish(winner));
    }

    IEnumerator DelayFinish(int winner)
    {
        yield return new WaitForSeconds(2f);
        OnMiniGameFinished?.Invoke(winner);
        IsFinished = true;
    }

    public void SetMiniGameTime(float time)
    {
        miniGameDuration = time;
    }

    private void SetFloatingButtonsActive(bool active)
    {
        FloatingButton[] floaters = FindObjectsOfType<FloatingButton>();
        foreach (var f in floaters)
            f.enabled = active;
    }
}
