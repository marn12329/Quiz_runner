using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class QuestionManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject questionPanel;
    public GameObject answerPanel;
    public GameObject resultPanel;
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI resultText;
    public Button[] answerButtons;

    [Header("Question Data")]
    public QuestionData[] questions;

    public Action<int, bool> OnAnswerResult;
    public Action OnAnswerFinished; // ✅ เพิ่ม event สำหรับบอกว่าตอบเสร็จ

    private int currentQuestionIndex = 0;
    private int currentPlayerIndex = -1;
    private int totalQuestions = 10;

    void Start()
    {
        if (questionPanel) questionPanel.SetActive(false);
        if (answerPanel) answerPanel.SetActive(false);
        if (resultPanel) resultPanel.SetActive(false);

        if (ScoreManager.Instance != null)
            ScoreManager.Instance.ResetScores();
    }

    public void ShowQuestion(int index)
    {
        if (index < 0 || index >= questions.Length)
        {
            Debug.LogError("Index คำถามไม่ถูกต้อง!");
            return;
        }

        currentQuestionIndex = index;
        questionPanel.SetActive(true);
        answerPanel.SetActive(false);
        if (resultPanel != null) resultPanel.SetActive(false);

        questionText.text = questions[index].question;
    }

    public void ShowAnswers(int questionIndex, int playerIndex)
    {
        if (questionIndex < 0 || questionIndex >= questions.Length)
        {
            Debug.LogError("Index คำถามไม่ถูกต้อง!");
            return;
        }

        currentPlayerIndex = playerIndex;
        var q = questions[questionIndex];

        answerPanel.SetActive(true);
        questionPanel.SetActive(false);

        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (i < q.answers.Length)
            {
                answerButtons[i].gameObject.SetActive(true);
                var txt = answerButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                if (txt != null) txt.text = q.answers[i];

                int answerIndex = i;
                answerButtons[i].onClick.RemoveAllListeners();
                answerButtons[i].onClick.AddListener(() => OnAnswerClicked(answerIndex, q.correctIndex));
            }
            else
            {
                answerButtons[i].gameObject.SetActive(false);
            }
        }
    }

    void OnAnswerClicked(int index, int correctIndex)
    {
        bool isCorrect = index == correctIndex;

        answerPanel.SetActive(false);
        questionPanel.SetActive(false);

        OnAnswerResult?.Invoke(currentPlayerIndex, isCorrect);

        if (isCorrect && currentPlayerIndex >= 0)
            ScoreManager.Instance.AddScore(currentPlayerIndex, 1);

        ShowResult(isCorrect);

        // ✅ แจ้ง GameFlowManager ว่าจบการตอบแล้ว
        OnAnswerFinished?.Invoke();
    }

    public void ShowResult(bool isCorrect)
    {
        if (resultPanel == null || resultText == null)
            return;

        resultPanel.SetActive(true);
        resultText.text = isCorrect ? " ตอบถูก" : " ตอบผิด";
        resultText.color = isCorrect ? Color.green : Color.red;

        Invoke(nameof(HideResult), 1.5f);
    }

    void HideResult()
    {
        if (resultPanel != null)
            resultPanel.SetActive(false);
    }
}

[System.Serializable]
public class QuestionData
{
    [TextArea(2, 5)]
    public string question;
    public string[] answers;
    public int correctIndex;
}
