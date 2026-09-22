using UnityEngine;

public class PlayerIndex : MonoBehaviour
{
    [Header("Player Info")]
    [Tooltip("Player ID ของผู้เล่นนี้ (0 = Player1, 1 = Player2, ...)")]
    public int playerID = -1;  // Player ID (0-3)

    private void Awake()
    {
        // ถ้ายังไม่ได้เซ็ต ID ลองดึงจากชื่อ GameObject เช่น "Player0", "Player1"
        if (playerID < 0)
        {
            TrySetPlayerIDFromName();
        }
    }

    private void Start()
    {
        // ตรวจสอบว่ามีชื่อผู้เล่นใน PlayerData หรือยัง ถ้ายังไม่มีให้โหลดจาก PlayerPrefs
        EnsurePlayerNameInData();
    }

    /// <summary>
    /// ตั้งค่า ID ของผู้เล่นจากภายนอก
    /// </summary>
    public void SetPlayerID(int id)
    {
        playerID = id;
    }

    /// <summary>
    /// ถ้า PlayerID ยังไม่ได้ตั้งค่า ลองอ่านจากชื่อ GameObject เช่น "Player0"
    /// </summary>
    private void TrySetPlayerIDFromName()
    {
        string objName = gameObject.name;
        if (objName.StartsWith("Player"))
        {
            string idStr = objName.Replace("Player", "");
            if (int.TryParse(idStr, out int parsedID))
            {
                playerID = parsedID;
            }
        }
    }

    /// <summary>
    /// ตรวจสอบให้แน่ใจว่า PlayerData มีชื่อผู้เล่นครบทุกคน
    /// </summary>
    private void EnsurePlayerNameInData()
    {
        if (PlayerData.playerNames == null || PlayerData.playerNames.Length < 4)
        {
            PlayerData.playerNames = new string[4];
        }

        // โหลดชื่อจาก PlayerPrefs ถ้ายังไม่ได้ตั้งไว้
        if (string.IsNullOrEmpty(PlayerData.playerNames[playerID]))
        {
            string savedName = PlayerPrefs.GetString($"Player{playerID + 1}_Name", $"Player {playerID + 1}");
            PlayerData.playerNames[playerID] = savedName;
        }
    }
}
