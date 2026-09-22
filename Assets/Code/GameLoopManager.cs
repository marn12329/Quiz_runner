using UnityEngine;
using System.Collections.Generic;

public class GameLoopManager : MonoBehaviour
{
    public List<PlayerMovement> players; // ลาก Player ทั้งหมดมาใส่ใน Inspector
    public float moveDistance = 2f;      // ระยะทางต่อ 1 คำตอบถูก

    // ฟังก์ชันจำลองให้ Player i เดินเมื่อถูก
    public void OnCorrectAnswer(int playerIndex)
    {
        if (playerIndex >= 0 && playerIndex < players.Count)
        {
            players[playerIndex].MoveForward(moveDistance);
        }
    }
}
