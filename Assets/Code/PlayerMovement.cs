using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Settings")]
    public string playerName = "Player";   // ชื่อผู้เล่น
    public float speed = 3f;               // ความเร็วเดิน

    private Vector3 targetPosition;
    private bool isMoving = false;
    private PlayerIndex playerIndex;       // เก็บ PlayerIndex เอาไว้

    void Start()
    {
        playerIndex = GetComponent<PlayerIndex>();
        LoadPlayerName();
        targetPosition = transform.position; // เริ่มที่ตำแหน่งปัจจุบัน
    }

    void Update()
    {
        HandleMovement();
    }

    /// <summary>
    /// จัดการการเคลื่อนที่ของผู้เล่น
    /// </summary>
    private void HandleMovement()
    {
        if (isMoving)
        {
            // เดินไปข้างหน้าอย่าง Smooth
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            // ถ้าถึงจุดเป้าหมายแล้วหยุดเดิน
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                isMoving = false;
            }
        }
    }

    /// <summary>
    /// สั่งให้ผู้เล่นเดินไปข้างหน้า
    /// </summary>
    public void MoveForward(float distance)
    {
        targetPosition += Vector3.right * distance;
        isMoving = true;
    }

    /// <summary>
    /// กำหนดชื่อผู้เล่นจากภายนอก
    /// </summary>
    public void SetPlayerName(string name)
    {
        playerName = name;
    }

    /// <summary>
    /// โหลดชื่อผู้เล่นอัตโนมัติจาก PlayerPrefs หรือ PlayerData
    /// </summary>
    private void LoadPlayerName()
    {
        if (playerIndex == null) return;

        string savedName = PlayerPrefs.GetString($"Player{playerIndex.playerID}_Name", "");
        if (!string.IsNullOrEmpty(savedName))
        {
            playerName = savedName;
        }
        else if (PlayerData.playerNames != null && playerIndex.playerID < PlayerData.playerNames.Length)
        {
            playerName = PlayerData.playerNames[playerIndex.playerID];
        }
    }
}
