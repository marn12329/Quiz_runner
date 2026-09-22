using UnityEngine;
using UnityEngine.UI;

public class LevelSelectScene : MonoBehaviour
{
    [Header("UI ปุ่มเลือกวิชา")]
    public Button mathButton;
    public Button thaiButton;
    public Button englishButton;

    void Start()
    {
        if (SceneFlowManager.Instance == null)
        {
            Debug.LogError("❌ SceneFlowManager not found in scene!");
            return;
        }

        // ตั้งค่า event ให้ปุ่มต่าง ๆ
        if (mathButton != null)
            mathButton.onClick.AddListener(() =>
            {
                SceneFlowManager.Instance.SelectSubject("Math");
                SceneFlowManager.Instance.GoToCharacterSelect();
            });

        if (thaiButton != null)
            thaiButton.onClick.AddListener(() =>
            {
                SceneFlowManager.Instance.SelectSubject("Thai");
                SceneFlowManager.Instance.GoToCharacterSelect();
            });

        if (englishButton != null)
            englishButton.onClick.AddListener(() =>
            {
                SceneFlowManager.Instance.SelectSubject("English");
                SceneFlowManager.Instance.GoToCharacterSelect();
            });
    }
}
