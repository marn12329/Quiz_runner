using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Linq;

public class ButtonMashGameManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI rankingText;  // UI สำหรับประกาศผล
    public ButtonMashUI buttonMashUI;
    public GameObject[] otherUI;  // ใส่ UI อื่นๆ ที่อยากให้หายไปเมื่อจบเกม

    [Header("Game Settings")]
    public float gameTime = 5f;  // เวลาเล่น 5 วินาที

    private float timeLeft;
    private bool gameRunning = false;
    private int[] playerScores = new int[4];  // เก็บคะแนนผู้เล่น 1-4

    void Start()
    {
        timeLeft = gameTime;
        timerText.text = "Press Start!";
        if (rankingText != null) rankingText.text = ""; // ซ่อนตอนเริ่ม
    }

    void Update()
    {
        if (!gameRunning) return;

        // นับเวลาถอยหลัง
        timeLeft -= Time.deltaTime;
        timerText.text = Mathf.Ceil(timeLeft).ToString();

        // จบเกมเมื่อหมดเวลา
        if (timeLeft <= 0)
        {
            EndGame();
        }
    }

    // เริ่มเกมใหม่
    public void StartGame()
    {
        for (int i = 0; i < playerScores.Length; i++)
            playerScores[i] = 0;

        timeLeft = gameTime;
        gameRunning = true;
        timerText.text = Mathf.Ceil(timeLeft).ToString();
        if (rankingText != null) rankingText.text = "";

        // เปิด UI อื่นๆ ตอนเริ่มเกม
        foreach (GameObject ui in otherUI)
        {
            ui.SetActive(true);
        }
    }

    // เพิ่มคะแนนให้ผู้เล่น
    public void AddScore(int playerIndex)
    {
        if (gameRunning && playerIndex >= 0 && playerIndex < playerScores.Length)
        {
            playerScores[playerIndex]++;
        }
    }

    // ตรวจสอบว่าเกมกำลังรันอยู่ไหม
    public bool IsGameRunning()
    {
        return gameRunning;
    }

    // สิ้นสุดเกมและประกาศผู้ชนะ
    private void EndGame()
    {
        gameRunning = false;
        timerText.text = "Time's Up!";

        // ซ่อน UI อื่นๆ
        foreach (GameObject ui in otherUI)
        {
            ui.SetActive(false);
        }

        // เรียงคะแนนมากไปน้อย
        var ranking = playerScores
            .Select((score, index) => new { Player = index, Score = score })
            .OrderByDescending(p => p.Score)
            .ToList();

        // แสดงผลอันดับ
        string resultText = " Results:\n";
        string orderString = "";  // เก็บลำดับผู้เล่นสำหรับ PlayerPrefs

        for (int i = 0; i < ranking.Count; i++)
        {
            resultText += (i + 1) + ". Player " + (ranking[i].Player + 1) + " - " + ranking[i].Score + " points\n";
            orderString += ranking[i].Player + (i < ranking.Count - 1 ? "," : "");
        }

        if (rankingText != null)
            rankingText.text = resultText;

        // บันทึกลำดับผู้เล่นไปใช้ใน CharacterSelectScene
        PlayerPrefs.SetString("PlayerOrder", orderString);

        // รอ 3 วินาทีก่อนเปลี่ยนซีน
        StartCoroutine(LoadNextSceneAfterDelay(3f));
    }

    IEnumerator LoadNextSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("CharacterSelectScene");
    }
}
