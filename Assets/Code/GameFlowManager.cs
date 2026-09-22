using UnityEngine;
using TMPro;
using System.Collections;
using System;

public class GameFlowManager : MonoBehaviour
{
    public static GameFlowManager Instance { get; private set; }

    [Header("UI Panels")]
    public GameObject countdownPanel;
    public TextMeshProUGUI countdownText;
    public GameObject questionPanel;
    public GameObject miniGamePanel;
    public GameObject answerPanel;
    public GameObject resultPanel;
    public GameObject winnerPanel;
    public TextMeshProUGUI winnerText;

    [Header("Managers")]
    public QuestionManager questionManager;
    public NewMiniGame newMiniGame;
    public ScoreManager scoreManager;

    public PlayerController[] players;
    public float countdownTime = 5f;
    public float questionTime = 5f;
    public float answerTime = 5f;
    public int totalQuestions = 5;
    public float winnerDisplayTime = 3f;

    private int currentQuestionIndex = 0;
    private bool gameEnded = false;
    private int lastWinnerPlayerID = -1;
    private bool answerReceived = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        if (players == null || players.Length == 0)
            players = FindObjectsOfType<PlayerController>();

        if (questionManager != null)
        {
            questionManager.OnAnswerResult += HandleAnswerResult;
            questionManager.OnAnswerFinished += HandleAnswerFinished;
        }
        else
        {
            Debug.LogWarning("QuestionManager not assigned in GameFlowManager inspector.");
        }

        HideAllPanels();
        StartCoroutine(GameLoop());
    }

    IEnumerator GameLoop()
    {
        while (!gameEnded)
        {
            yield return ShowCountdown();
            yield return ShowQuestion();
            yield return PlayMiniGame();
            yield return ShowWinnerBeforeAnswer();
            yield return AnswerPhase();
            yield return ShowResult();

            PlayerData.TickEndOfRound();
            currentQuestionIndex++;
            if (currentQuestionIndex >= totalQuestions)
                gameEnded = true;
        }

        yield return EndGameSequence();
    }

    IEnumerator ShowCountdown()
    {
        HideAllPanels();
        if (countdownPanel != null) countdownPanel.SetActive(true);
        float t = countdownTime;
        while (t > 0)
        {
            if (countdownText != null) countdownText.text = Mathf.Ceil(t).ToString();
            t -= Time.deltaTime;
            yield return null;
        }
        if (countdownPanel != null) countdownPanel.SetActive(false);
    }

    IEnumerator ShowQuestion()
    {
        HideAllPanels();
        if (questionPanel != null)
        {
            questionPanel.SetActive(true);
            questionManager.ShowQuestion(currentQuestionIndex);
            yield return new WaitForSeconds(questionTime);
            questionPanel.SetActive(false);
        }
        else
        {
            yield return null;
        }
    }

    IEnumerator PlayMiniGame()
    {
        HideAllPanels();
        if (miniGamePanel != null) miniGamePanel.SetActive(true);

        bool finished = false;
        int winnerQuadrant = -1;

        if (newMiniGame != null)
        {
            newMiniGame.OnMiniGameFinished += (winner) =>
            {
                winnerQuadrant = winner;
                finished = true;
            };

            newMiniGame.StartMiniGame(lastWinnerPlayerID);

            while (!finished)
                yield return null;

            lastWinnerPlayerID = winnerQuadrant;
            yield return new WaitForSeconds(1f);
        }
        else
        {
            Debug.LogWarning("NewMiniGame not assigned. Skipping mini-game step.");
            yield return new WaitForSeconds(0.5f);
        }

        if (miniGamePanel != null) miniGamePanel.SetActive(false);
    }

    IEnumerator ShowWinnerBeforeAnswer()
    {
        HideAllPanels();
        if (winnerPanel != null) winnerPanel.SetActive(true);

        string name = (lastWinnerPlayerID >= 0 && lastWinnerPlayerID < players.Length)
            ? players[lastWinnerPlayerID].playerName
            : "???";

        if (winnerText != null) winnerText.text = $"ผู้ชนะรอบนี้คือ {name}";

        yield return new WaitForSeconds(winnerDisplayTime);
        if (winnerPanel != null) winnerPanel.SetActive(false);
    }

    IEnumerator AnswerPhase()
    {
        HideAllPanels();
        if (answerPanel != null) answerPanel.SetActive(true);

        answerReceived = false;
        questionManager.ShowAnswers(currentQuestionIndex, lastWinnerPlayerID);

        while (!answerReceived)
            yield return null;

        if (answerPanel != null) answerPanel.SetActive(false);
    }

    // เมื่อได้คำตอบจากผู้เล่น
    void HandleAnswerResult(int playerIndex, bool isCorrect)
    {
        if (isCorrect)
        {
            // ตอบถูก — เดิน + รับไอเท็มใหม่
            PlayerController target = Array.Find(players, p => p.playerID == playerIndex);
            if (target != null)
            {
                target.StartRunning();

                if (PlayerData.doubleRunCounts[playerIndex] > 0)
                {
                    PlayerData.doubleRunCounts[playerIndex]--;
                    target.StartRunning();
                }

                StartCoroutine(GiveItemAfterDelay(playerIndex));
            }

            // ได้ไอเท็มใหม่ → ล้างสถานะทุกคน
            PlayerController.ClearAllForNewState();
        }
        else
        {
            // ตอบผิด → รีเซ็ตทุกคน
            Debug.Log($"Player {playerIndex} ตอบผิด — รีเซ็ตทุกคน!");
            PlayerController.ClearAllForNewState();
        }
    }

    void HandleAnswerFinished()
    {
        answerReceived = true;
        if (answerPanel != null) answerPanel.SetActive(false);
    }

    IEnumerator GiveItemAfterDelay(int id)
    {
        yield return new WaitForSeconds(1.5f);
        if (ItemSystemManager.Instance != null)
            yield return ItemSystemManager.Instance.OfferRandomItemToPlayer(id);
    }

    IEnumerator ShowResult()
    {
        HideAllPanels();
        if (resultPanel != null) resultPanel.SetActive(true);
        yield return new WaitForSeconds(2f);
        if (resultPanel != null) resultPanel.SetActive(false);
    }

    IEnumerator EndGameSequence()
    {
        HideAllPanels();
        yield return new WaitForSeconds(1f);
        if (scoreManager != null)
        {
            if (resultPanel != null) resultPanel.SetActive(true);
            scoreManager.UpdateScoreboard();
        }
    }

    void HideAllPanels()
    {
        countdownPanel?.SetActive(false);
        questionPanel?.SetActive(false);
        answerPanel?.SetActive(false);
        resultPanel?.SetActive(false);
        winnerPanel?.SetActive(false);
        miniGamePanel?.SetActive(false);
    }
}
