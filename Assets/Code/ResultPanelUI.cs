using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResultPanelUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject panelRoot;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI[] rankTexts;
    public Button restartButton;

    void Start()
    {
        if (panelRoot != null)
            panelRoot.SetActive(false);

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);
    }

    public void ShowResults()
    {
        if (panelRoot == null) return;

        panelRoot.SetActive(true);
        titleText.text = "🏆 ผลการแข่งขัน";

        string[] names = PlayerData.playerNames;
        int[] scores = ScoreManager.Instance.playerScores;

        int[] sortedIndex = { 0, 1, 2, 3 };
        System.Array.Sort(sortedIndex, (a, b) => scores[b].CompareTo(scores[a]));

        for (int i = 0; i < rankTexts.Length; i++)
        {
            int idx = sortedIndex[i];
            if (i < rankTexts.Length && rankTexts[i] != null)
                rankTexts[i].text = $"{i + 1}. {PlayerData.GetPlayerName(idx)} - {scores[idx]} คะแนน";
        }
    }

    private void RestartGame()
    {
        string sceneToLoad;

        // ✅ เปลี่ยนชื่อซีนให้ตรงกับที่คุณใช้จริง
        switch (PlayerData.CurrentSubject)
        {
            case PlayerData.SubjectType.Math:
                sceneToLoad = "MiniGameScene1";
                break;
            case PlayerData.SubjectType.Thai:
                sceneToLoad = "MiniGameScene2";
                break;
            case PlayerData.SubjectType.English:
                sceneToLoad = "MiniGameScene3";
                break;
            default:
                sceneToLoad = SceneManager.GetActiveScene().name;
                Debug.LogWarning("⚠️ ไม่พบข้อมูลวิชาปัจจุบันใน PlayerData — โหลดซีนเดิมแทน");
                break;
        }

        Debug.Log($"🔁 Restart game to scene: {sceneToLoad}");
        SceneManager.LoadScene(sceneToLoad);
    }
}
