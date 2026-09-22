using UnityEngine;
using TMPro;

public class PlayerNameUI : MonoBehaviour
{
    [Header("Name Settings")]
    public Vector3 nameOffset = new Vector3(0, 2f, 0);  // ระยะห่างจากหัวผู้เล่น

    private Camera mainCamera;
    private TextMeshProUGUI nameText;
    private RectTransform nameRect;
    private Canvas nameCanvas;
    private PlayerController playerController;
    private PlayerIndex playerIndex;

    void Start()
    {
        mainCamera = Camera.main;
        playerController = GetComponent<PlayerController>();
        playerIndex = GetComponent<PlayerIndex>();

        CreateNameCanvas();
        CreateNameText();

        nameRect.sizeDelta = new Vector2(2000, 50);
        nameCanvas.transform.localPosition = nameOffset;

        // ✅ รอให้ระบบอื่นเซ็ตค่าเสร็จก่อน แล้วอัปเดตชื่อ
        Invoke(nameof(UpdatePlayerName), 0.2f);
    }

    void LateUpdate()
    {
        if (mainCamera != null && nameRect != null)
        {
            nameRect.transform.rotation = Quaternion.LookRotation(
                nameRect.transform.position - mainCamera.transform.position
            );
        }
    }

    private void CreateNameCanvas()
    {
        GameObject canvasObj = new GameObject("NameCanvas");
        canvasObj.transform.SetParent(transform, false);
        nameCanvas = canvasObj.AddComponent<Canvas>();
        nameCanvas.renderMode = RenderMode.WorldSpace;
        nameCanvas.worldCamera = mainCamera;
        nameCanvas.sortingOrder = 10;
        canvasObj.transform.localScale = Vector3.one * 0.01f;
    }

    private void CreateNameText()
    {
        GameObject textObj = new GameObject("NameText");
        textObj.transform.SetParent(nameCanvas.transform, false);
        nameText = textObj.AddComponent<TextMeshProUGUI>();
        nameText.fontSize = 100;
        nameText.alignment = TextAlignmentOptions.Center;
        nameText.color = Color.white;
        nameRect = nameText.GetComponent<RectTransform>();
    }

    // ✅ ใช้เรียกตอนเริ่มเพื่ออัปเดตชื่อผู้เล่น
    private void UpdatePlayerName()
    {
        if (nameText == null) return;

        string finalName = GetPlayerName();
        nameText.text = string.IsNullOrEmpty(finalName) ? gameObject.name : finalName;
    }

    // ✅ ดึงชื่อจาก PlayerController เป็นหลัก
    private string GetPlayerName()
    {
        // 1️⃣ ใช้ชื่อจาก PlayerController ก่อน
        if (playerController != null && !string.IsNullOrEmpty(playerController.playerName))
            return playerController.playerName;

        // 2️⃣ ถ้ามี PlayerData และ PlayerIndex ใช้ fallback
        if (playerIndex != null && PlayerData.playerNames != null &&
            playerIndex.playerID < PlayerData.playerNames.Length &&
            !string.IsNullOrEmpty(PlayerData.playerNames[playerIndex.playerID]))
        {
            return PlayerData.playerNames[playerIndex.playerID];
        }

        // 3️⃣ fallback สุดท้าย
        return gameObject.name;
    }
}
