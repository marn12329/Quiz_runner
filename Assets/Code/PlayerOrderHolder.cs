using System.Collections.Generic;
using UnityEngine;

public class PlayerOrderHolder : MonoBehaviour
{
    public static PlayerOrderHolder I;

    // ลำดับจากมินิเกมแย่งกด (ค่าคือ playerId 0..3)
    public List<int> playerOrder = new();
    // ตัวละครที่เลือกต่อผู้เล่น (index 0..3) ค่า -1 คือยังไม่เลือก
    public int[] selectedCharacterByPlayer = new int[4];
    // คะแนน/สถิติระหว่างเกมหลัก
    public int[] playerScores = new int[4];

    private void Awake()
    {
        if (I == null) { I = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
        for (int i = 0; i < selectedCharacterByPlayer.Length; i++)
            if (selectedCharacterByPlayer[i] == 0) selectedCharacterByPlayer[i] = -1;
    }

    public void ResetForNewRun()
    {
        playerOrder.Clear();
        for (int i = 0; i < 4; i++)
        {
            selectedCharacterByPlayer[i] = -1;
            playerScores[i] = 0;
        }
    }
}
