using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFlowManager : MonoBehaviour
{
    public static SceneFlowManager Instance { get; private set; }

    [Header("Scene Names")]
    public string lobbyScene = "LobbyScene";
    public string nameInputScene = "NameInputScene";
    public string levelSelectScene = "LevelSelectScene";

    [Header("Dynamic Scene Names (Auto Set After Selecting Subject)")]
    public string selectedCharacterScene;
    public string selectedMiniGameScene;

    [Header("Debug Info")]
    public string currentSubject = "None";


    // ============================================
    // INITIALIZATION
    // ============================================
    void Awake()
    {
        // ป้องกันซ้ำเมื่อเปลี่ยนซีน (ต้องมีแค่ตัวเดียวเท่านั้น)
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"⚠ Duplicate SceneFlowManager detected in scene '{gameObject.scene.name}'. Destroying new one.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Debug.Log($"✅ SceneFlowManager initialized from scene: {gameObject.scene.name}");
    }


    // ============================================
    // UTILITIES
    // ============================================
    private bool LoadSceneSafe(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("❌ Scene name is EMPTY.");
            return false;
        }

        // ตรวจว่าซีนถูก Add ไปใน Build Settings หรือยัง
        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError(
                $"❌ Scene '{sceneName}' ยังไม่ได้ Add ใน Build Settings!\n" +
                $"👉 File → Build Settings → Add Open Scenes"
            );
            return false;
        }

        Debug.Log($"➡ Loading Scene: {sceneName}");
        SceneManager.LoadScene(sceneName);
        return true;
    }


    // ============================================
    // SCENE FLOW FUNCTIONS
    // ============================================
    public void GoToNameInput()
    {
        Debug.Log("🎬 Go → NameInputScene");
        LoadSceneSafe(nameInputScene);
    }

    public void GoToLevelSelect()
    {
        Debug.Log("🎯 Go → LevelSelectScene");
        LoadSceneSafe(levelSelectScene);
    }

    // --------------------------------------------
    // เลือกวิชา → เซ็ตซีนที่จะใช้ในขั้นตอนถัดไป
    // --------------------------------------------
    public void SelectSubject(string subject)
    {
        currentSubject = subject;

        switch (subject)
        {
            case "Math":
                selectedCharacterScene = "CharacterSelectScene1";
                selectedMiniGameScene = "MiniGameScene1";
                break;

            case "Thai":
                selectedCharacterScene = "CharacterSelectScene2";
                selectedMiniGameScene = "MiniGameScene2";
                break;

            case "English":
                selectedCharacterScene = "CharacterSelectScene3";
                selectedMiniGameScene = "MiniGameScene3";
                break;

            default:
                Debug.LogError($"❌ Unknown subject: {subject}");
                return;
        }

        PlayerData.SetSubject(subject);
        Debug.Log($"✅ Subject Selected: {subject}");
    }


    public void GoToCharacterSelect()
    {
        if (string.IsNullOrEmpty(selectedCharacterScene))
        {
            Debug.LogError("❌ Can't load character select. Subject not selected.");
            return;
        }

        Debug.Log("👤 Go → Character Select Scene");
        LoadSceneSafe(selectedCharacterScene);
    }


    public void GoToMiniGame()
    {
        if (string.IsNullOrEmpty(selectedMiniGameScene))
        {
            Debug.LogError("❌ Can't load mini game. Subject not selected.");
            return;
        }

        Debug.Log("🎮 Go → MiniGame");
        LoadSceneSafe(selectedMiniGameScene);
    }


    public void ReturnToLevelSelect()
    {
        Debug.Log("↩ Back → LevelSelectScene");
        LoadSceneSafe(levelSelectScene);
    }

    public void GoToLobby()
    {
        Debug.Log("🏠 Back → LobbyScene");
        LoadSceneSafe(lobbyScene);
    }
}
