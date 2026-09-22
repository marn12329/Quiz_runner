using UnityEngine;
using System.Collections;

public class PlayerItemHandler : MonoBehaviour
{
    [Header("Current Item")]
    public ItemData currentItem;

    [Header("Player Index (0-3)")]
    public int playerIndex;

    private PlayerController playerController;

    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        if (playerController != null)
            playerIndex = playerController.playerID;
    }

    public void AddItem(ItemData newItem)
    {
        currentItem = newItem;
        Debug.Log($"{PlayerData.GetPlayerName(playerIndex)} ได้รับไอเท็ม: {newItem.itemName}");
        UseItem();
    }

    public void UseItem()
    {
        if (currentItem == null)
        {
            Debug.LogWarning("ไม่มีไอเท็มให้ใช้");
            return;
        }

        Debug.Log($"{PlayerData.GetPlayerName(playerIndex)} ใช้ไอเท็ม: {currentItem.itemName}");

        switch (currentItem.effectType)
        {
            case ItemEffectType.FreezeOthers:
                ApplyFreezeOthers();
                break;

            case ItemEffectType.SlowOthers:
                StartCoroutine(SlowOthersEffect());
                break;

            case ItemEffectType.DoubleRun:
                PlayerData.AddDoubleRun(playerIndex, 1);
                playerController.ApplySpeedMultiplier(2f);
                Debug.Log("⚡ เพิ่มความเร็ววิ่ง 2 เท่า");
                break;

            case ItemEffectType.SkipTurn:
                PlayerData.SetSkipNextTurn(playerIndex, true);
                Debug.Log($"⏭ Player {playerIndex} จะถูกข้ามเทิร์นถัดไป");
                break;

            default:
                Debug.Log("ไม่มีเอฟเฟกต์สำหรับไอเท็มนี้");
                break;
        }

        currentItem = null;
    }

    private void ApplyFreezeOthers()
    {
        int target = GetRandomOtherPlayer(playerIndex);
        if (target != -1)
        {
            PlayerData.SetSkipNextTurn(target, true); // ใช้ระบบเดียวกับ SkipTurn
            Debug.Log($"🧊 Player {playerIndex + 1} แช่ Player {target + 1}!");
        }
        else
        {
            Debug.Log("ไม่มีผู้เล่นคนอื่นให้ Freeze");
        }
    }

    private IEnumerator SlowOthersEffect()
    {
        Debug.Log("🐌 SlowOthers: ลดความเร็วผู้เล่นอื่น 50%");

        var players = FindObjectsOfType<PlayerController>();
        foreach (var player in players)
        {
            if (player == null || player.playerID == playerIndex) continue;
            player.ApplySpeedMultiplier(0.5f);
        }

        yield return new WaitForSeconds(5f);

        foreach (var player in players)
        {
            if (player == null) continue;
            player.ResetSpeed();
        }

        Debug.Log("✅ ผู้เล่นทุกคนกลับมาความเร็วปกติ");
    }

    private int GetRandomOtherPlayer(int exclude)
    {
        int total = PlayerData.playerNames.Length;
        int target = Random.Range(0, total);
        while (target == exclude && total > 1)
            target = Random.Range(0, total);
        return target;
    }
}
