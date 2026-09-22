using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ItemSystemManager : MonoBehaviour
{
    public static ItemSystemManager Instance;

    [Header("UI ของ Popup (ลาก ItemPopupUI มาวางใน Inspector)")]
    public ItemPopupUI itemPopupUI;

    [Header("รายการ Item ที่จะสุ่ม")]
    public List<ItemData> availableItems = new List<ItemData>();

    [Header("Score Setting")]
    public int doubleRunScore = 1;   // ✅ คะแนนที่ได้จาก DoubleRun (ปรับได้ใน Inspector)

    private bool choiceMade = false;
    private bool accepted = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // ✅ เรียกจาก GameFlowManager หลังตอบคำถามถูก
    public IEnumerator OfferRandomItemToPlayer(int playerID)
    {
        if (availableItems.Count == 0 || itemPopupUI == null)
        {
            Debug.LogWarning("⚠️ ItemSystemManager: ไม่มีรายการไอเท็มหรือ UI ยังไม่เซ็ต");
            yield break;
        }

        // หยุดเวลาเกมชั่วคราว
        Time.timeScale = 0f;

        // ✅ สุ่มไอเท็ม
        ItemData chosen = availableItems[UnityEngine.Random.Range(0, availableItems.Count)];
        choiceMade = false;
        accepted = false;

        // ✅ แสดง popup ถามว่าจะรับไหม
        itemPopupUI.ShowAsk(
            chosen,
            onAccept: () => { accepted = true; choiceMade = true; },
            onDecline: () => { accepted = false; choiceMade = true; }
        );

        yield return new WaitUntil(() => choiceMade);

        if (!accepted)
        {
            itemPopupUI.HideInstant();
            Time.timeScale = 1f;
            yield break;
        }

        // ✅ แสดงผลรับไอเท็ม
        yield return itemPopupUI.ShowReceived(chosen, 1.5f);

        Time.timeScale = 1f;

        // ✅ ก่อนมอบของ — ล้างทุกคน
        PlayerController.ClearAllForNewState();

        // ✅ มอบไอเท็ม
        bool given = GiveItemToPlayerHandler(playerID, chosen);
        if (!given)
        {
            ApplyItemEffectToPlayer(playerID, chosen);
        }

        itemPopupUI.HideInstant();
    }

    private bool GiveItemToPlayerHandler(int playerID, ItemData item)
    {
        PlayerController[] allPlayers = FindObjectsOfType<PlayerController>();
        if (allPlayers != null && allPlayers.Length > 0)
        {
            foreach (var pc in allPlayers)
            {
                if (pc != null && pc.playerID == playerID)
                {
                    var handler = pc.GetComponent<PlayerItemHandler>();
                    if (handler != null)
                    {
                        handler.AddItem(item);
                        Debug.Log($"✅ มอบ \"{item.itemName}\" ให้ {PlayerData.GetPlayerName(playerID)} ผ่าน PlayerItemHandler.");
                        ApplyItemEffectToPlayer(playerID, item);
                        return true;
                    }
                }
            }
        }

        if (GameFlowManager.Instance != null && GameFlowManager.Instance.players != null)
        {
            foreach (var pc in GameFlowManager.Instance.players)
            {
                if (pc != null && pc.playerID == playerID)
                {
                    var handler = pc.GetComponent<PlayerItemHandler>();
                    if (handler != null)
                    {
                        handler.AddItem(item);
                        Debug.Log($"✅ มอบ \"{item.itemName}\" ให้ {PlayerData.GetPlayerName(playerID)} ผ่าน GameFlowManager.players.");
                        ApplyItemEffectToPlayer(playerID, item);
                        return true;
                    }
                }
            }
        }

        Debug.LogWarning($"❌ ItemSystemManager: ไม่พบ PlayerID={playerID} สำหรับมอบไอเท็ม");
        return false;
    }

    private void ApplyItemEffectToPlayer(int playerID, ItemData item)
    {
        Debug.Log($"🎁 Player {playerID} ได้ไอเท็ม {item.itemName} ({item.effectType})");

        switch (item.effectType)
        {
            case ItemEffectType.FreezeOthers:
                int target = GetRandomOtherPlayer(playerID);
                if (target != -1)
                {
                    PlayerData.ApplyFreezeToPlayer(target, 1);
                    Debug.Log($"🧊 Freeze Player {target} โดย Player {playerID}");
                }
                break;

            case ItemEffectType.SlowOthers:
                for (int i = 0; i < PlayerData.playerNames.Length; i++)
                {
                    if (i == playerID) continue;
                    PlayerData.ApplySlowToPlayer(i, 1);
                }
                Debug.Log($"🐌 Player {playerID} ใช้ Slow Others ใส่ผู้เล่นอื่นทุกคน");
                break;

            case ItemEffectType.DoubleRun:

                // ✅ เพิ่มสิทธิ์วิ่งฟรี
                PlayerData.AddDoubleRun(playerID, 1);
                Debug.Log($"✨ Player {playerID} ได้สถานะ Double Run");

                var pcs = FindObjectsOfType<PlayerController>();
                foreach (var p in pcs)
                {
                    if (p != null && p.playerID == playerID)
                    {
                        // ✅ สั่งวิ่งทันที
                        p.StartRunning();
                        Debug.Log($"🏃‍♂️ Player {playerID} วิ่งฟรีทันทีเพราะ Double Run!");

                        // ✅ เพิ่มคะแนนเข้า Scoreboard
                        if (ScoreManager.Instance != null)
                        {
                            ScoreManager.Instance.AddScore(playerID, doubleRunScore);
                            Debug.Log($"🏆 Player {playerID} ได้คะแนนจาก Double Run +{doubleRunScore}");
                        }
                        else
                        {
                            Debug.LogWarning("⚠️ ScoreManager ยังไม่มีในฉาก");
                        }

                        break;
                    }
                }

                // ✅ เคลียร์สถานะหลังใช้
                PlayerData.doubleRunCounts[playerID] = 0;
                break;

            case ItemEffectType.SkipTurn:
                PlayerData.SetSkipNextTurn(playerID, true);
                Debug.Log($"⏭ Player {playerID} จะถูกข้ามเทิร์นถัดไป");
                break;
        }
    }

    private int GetRandomOtherPlayer(int exclude)
    {
        List<int> others = new List<int>();
        for (int i = 0; i < PlayerData.playerNames.Length; i++)
            if (i != exclude) others.Add(i);

        if (others.Count == 0) return -1;
        return others[UnityEngine.Random.Range(0, others.Count)];
    }

    // ✅ เรียกตอน "ตอบผิด"
    public void OnPlayerAnsweredWrong(int playerID)
    {
        if (playerID < 0 || playerID >= PlayerData.playerNames.Length)
        {
            Debug.LogWarning($"⚠️ Player ID {playerID} ไม่ถูกต้อง");
            return;
        }

        Debug.Log($"💥 Player {playerID} ตอบผิด — รีเซ็ตทุกคน!");
        PlayerController.ClearAllForNewState();
    }
}

[Serializable]
public class ItemData
{
    public string itemName;
    public Sprite icon;
    [TextArea] public string description;
    public ItemEffectType effectType;
}

public enum ItemEffectType
{
    None,
    FreezeOthers,
    SlowOthers,
    DoubleRun,
    SkipTurn
}
