using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PlayerNameInputManager : MonoBehaviour
{
    [Header("Input Fields (Player 1–4)")]
    public TMP_InputField[] nameInputs;

    [Header("UI Buttons")]
    public Button confirmButton;

    [Header("Feedback")]
    public TextMeshProUGUI statusText;

    [Header("On-Screen Keyboard")]
    public OnScreenKeyboardUI onScreenKeyboard; // ✅ อ้างถึงคีย์บอร์ดจอ

#if UNITY_ANDROID || UNITY_IOS || UNITY_TVOS
    private TouchScreenKeyboard keyboard;
#endif

    void Start()
    {
        // ✅ สร้าง EventSystem ถ้ายังไม่มี
        if (FindObjectOfType<EventSystem>() == null)
        {
            var es = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            Debug.Log("🧩 EventSystem ถูกสร้างอัตโนมัติ");
        }

        // ✅ โหลดชื่อเก่าจาก PlayerData
        PlayerData.LoadAllNames();

        for (int i = 0; i < nameInputs.Length; i++)
        {
            nameInputs[i].text = PlayerData.GetPlayerName(i);
            int index = i;
            nameInputs[i].onSelect.AddListener((value) => OpenKeyboard(index));
        }

        if (confirmButton != null)
        {
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(OnConfirmPressed);
        }
        else
        {
            Debug.LogError("❌ confirmButton ยังไม่ได้ Assign ใน Inspector!");
        }
    }

#if UNITY_ANDROID || UNITY_IOS || UNITY_TVOS
    void Update()
    {
        if (keyboard != null && keyboard.active)
        {
            for (int i = 0; i < nameInputs.Length; i++)
            {
                if (nameInputs[i].isFocused)
                    nameInputs[i].text = keyboard.text;
            }
        }
    }
#endif

    private void OpenKeyboard(int index)
    {
#if UNITY_ANDROID || UNITY_IOS || UNITY_TVOS
        // ✅ มือถือ ใช้ Touch Keyboard ปกติ
        string currentText = nameInputs[index].text;
        keyboard = TouchScreenKeyboard.Open(
            currentText,
            TouchScreenKeyboardType.Default,
            false, false, false, false,
            $"ชื่อผู้เล่น {index + 1}"
        );
#else
        // ✅ TV / PC ใช้ On-Screen Keyboard แทน
        if (onScreenKeyboard != null)
        {
            onScreenKeyboard.gameObject.SetActive(true);
            onScreenKeyboard.inputField = nameInputs[index];
            Debug.Log($"🎹 เปิดคีย์บอร์ดจอสำหรับ Player {index + 1}");
        }
        else
        {
            Debug.LogWarning("⚠️ ไม่มี OnScreenKeyboardUI ถูก Assign ใน Inspector");
        }
#endif
    }

    public void OnConfirmPressed()
    {
        for (int i = 0; i < nameInputs.Length; i++)
        {
            string inputName = nameInputs[i].text.Trim();
            if (string.IsNullOrEmpty(inputName))
                inputName = $"Player {i + 1}";

            PlayerData.SetPlayerName(i, inputName);
        }

        PlayerPrefs.Save();

        if (statusText != null)
            statusText.text = "✅ บันทึกชื่อเรียบร้อย!";

        StartCoroutine(NextScene());
    }

    private IEnumerator NextScene()
    {
        yield return new WaitForSeconds(0.5f);

        if (SceneFlowManager.Instance != null)
            SceneFlowManager.Instance.GoToLevelSelect();
        else
        {
            Debug.LogWarning("⚠️ SceneFlowManager ยังไม่ถูกสร้าง — โหลด LevelSelectScene แทน");
            SceneManager.LoadScene("LevelSelectScene");
        }
    }
}
