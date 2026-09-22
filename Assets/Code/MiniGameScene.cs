using UnityEngine;
using UnityEngine.UI;

public class MiniGameScene : MonoBehaviour
{
    public Text subjectText;

    void Start()
    {
        string subject = "Unknown";
        if (SceneFlowManager.Instance != null && !string.IsNullOrEmpty(SceneFlowManager.Instance.currentSubject))
            subject = SceneFlowManager.Instance.currentSubject;
        else
        {
            // fallback to PlayerData's last subject if available
            PlayerData.LoadLastSubject();
            subject = PlayerData.CurrentSubject.ToString();
        }

        if (subjectText != null)
            subjectText.text = $"🎮 วิชา: {subject}";

        Debug.Log($"✅ เข้ามาใน MiniGameScene ของวิชา: {subject}");
    }
}
