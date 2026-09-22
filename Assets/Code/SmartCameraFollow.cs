using UnityEngine;
using System.Linq;

public class SmartCameraFollow : MonoBehaviour
{
    [Header("Auto Player Detection")]
    public bool autoFindPlayers = true;
    public string playerTag = "Player";

    [Header("Camera Settings")]
    public float smoothSpeed = 4f;
    public Vector3 offset = new Vector3(0, 2, -10);

    [Header("Limit Settings")]
    public float minX = 0f;
    public float maxX = 100f;

    private Transform[] players;

    void Start()
    {
        if (autoFindPlayers)
            FindPlayersInScene();
    }

    void LateUpdate()
    {
        // หา Player ใหม่ถ้ายังไม่มี
        if (autoFindPlayers && (players == null || players.Length == 0))
            FindPlayersInScene();

        if (players == null || players.Length == 0)
            return;

        // 🏃‍♂️ หาผู้เล่นที่อยู่ไกลสุด (X มากสุด)
        Transform leader = players[0];
        foreach (Transform p in players)
        {
            if (p != null && p.position.x > leader.position.x)
                leader = p;
        }

        // 🎯 ตำแหน่งเป้าหมาย (ใช้ตำแหน่งของ leader จริง ๆ)
        float targetX = Mathf.Clamp(leader.position.x, minX, maxX);
        Vector3 targetPos = new Vector3(targetX, leader.position.y, 0f) + offset;

        // 🎥 เคลื่อนกล้องอย่างนุ่มนวล
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * smoothSpeed);
    }

    private void FindPlayersInScene()
    {
        GameObject[] foundPlayers = GameObject.FindGameObjectsWithTag(playerTag);

        if (foundPlayers.Length > 0)
        {
            players = foundPlayers.Select(p => p.transform).ToArray();
            Debug.Log($"🎯 Found {players.Length} players for camera tracking");
        }
        else
        {
            players = null;
            Debug.LogWarning("⚠️ SmartCameraFollow: ไม่พบผู้เล่นในฉาก (Tag: Player)");
        }
    }
}
