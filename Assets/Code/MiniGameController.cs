using UnityEngine;
using TMPro;
using System.Collections;

public class MiniGameController : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI resultText;

    [Header("MiniGame Settings")]
    public float miniGameDuration = 10f;

    private float timer;
    private bool isRunning = false;
    private bool winnerDeclared = false;

    public System.Action OnTimeUp;

    void Start()
    {
        ResetUI();
    }

    void Update()
    {
        if (!isRunning) return;

        timer -= Time.deltaTime;
        if (timer < 0f) timer = 0f;
        UpdateTimerUI(timer);

        if (timer <= 0f && !winnerDeclared)
        {
            EndMiniGame();
        }
    }

    public void StartMiniGame()
    {
        timer = miniGameDuration;
        isRunning = true;
        winnerDeclared = false;
        resultText.text = "เกมเริ่มแล้ว!";
        UpdateTimerUI(timer);
    }

    public void EndMiniGame()
    {
        if (!isRunning || winnerDeclared) return;

        isRunning = false;
        winnerDeclared = true;
        StartCoroutine(ShowTimeUp());
    }

    private IEnumerator ShowTimeUp()
    {
        resultText.text = "หมดเวลา!";
        yield return new WaitForSeconds(1f);
        OnTimeUp?.Invoke();
    }

    private void UpdateTimerUI(float timeValue)
    {
        if (timerText != null)
        {
            int seconds = Mathf.CeilToInt(timeValue);
            timerText.text = $"เวลา: {seconds} วิ";
        }
    }

    private void ResetUI()
    {
        UpdateTimerUI(miniGameDuration);
        if (resultText != null)
            resultText.text = "พร้อมเริ่มเกม!";
    }
}
