using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class PlayerSpawner : MonoBehaviour
{
    [Header("Math Prefabs (P1–P4)")]
    public GameObject[] mathPrefabs;

    [Header("Thai Prefabs (T1–T4)")]
    public GameObject[] thaiPrefabs;

    [Header("English Prefabs (E1–E4)")]
    public GameObject[] englishPrefabs;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    private List<PlayerController> spawnedPlayers = new List<PlayerController>();

    void Start()
    {
        StartCoroutine(SpawnAfterDelay());
    }

    IEnumerator SpawnAfterDelay()
    {
        yield return null; // รอให้ PlayerData โหลดเสร็จก่อน
        SpawnPlayers();
    }

    void SpawnPlayers()
    {
        PlayerData.LoadLastSubject();
        string subject = PlayerData.GetSubject();
        GameObject[] prefabsToUse = GetPrefabsForSubject(subject);

        if (prefabsToUse == null || prefabsToUse.Length == 0)
        {
            Debug.LogError($"❌ ไม่มี prefab สำหรับวิชา: {subject}");
            return;
        }

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            // ✅ โหลดชื่อ character จาก PlayerData
            string charName = PlayerData.GetPlayerCharacter(i);
            GameObject prefab = FindPrefabByName(prefabsToUse, charName);

            // ✅ fallback ถ้าไม่มี prefab ชื่อนี้
            if (prefab == null)
            {
                int safeIndex = Mathf.Clamp(i, 0, prefabsToUse.Length - 1);
                prefab = prefabsToUse[safeIndex];
                Debug.LogWarning($"⚠️ ไม่เจอ prefab '{charName}', ใช้ตัวสำรอง '{prefab.name}'");
            }

            // ✅ spawn player
            GameObject player = Instantiate(prefab, spawnPoints[i].position, Quaternion.identity);
            player.name = $"Player_{i + 1}_{prefab.name}";

            // ✅ ตั้งค่า controller
            PlayerController controller = player.GetComponent<PlayerController>();
            if (controller != null)
            {
                controller.playerID = i;
                controller.playerName = PlayerData.GetPlayerName(i);
                spawnedPlayers.Add(controller);
            }

            // ✅ ตั้งชื่อบน UI
            TextMeshProUGUI nameUI = player.GetComponentInChildren<TextMeshProUGUI>(true);
            if (nameUI != null)
                nameUI.text = controller != null ? controller.playerName : PlayerData.GetPlayerName(i);

            // ✅ จดทะเบียน prefab
            CharacterLookup.RegisterPrefab(prefab);
        }

        // ✅ ส่งข้อมูลให้ GameFlowManager
        GameFlowManager manager = FindObjectOfType<GameFlowManager>();
        if (manager != null)
            manager.players = spawnedPlayers.ToArray();

        Debug.Log($"✅ Spawn เสร็จเรียบร้อย ({subject}) จำนวน {spawnedPlayers.Count} ตัว");
    }

    GameObject[] GetPrefabsForSubject(string subject)
    {
        switch (subject)
        {
            case "Thai": return thaiPrefabs;
            case "English": return englishPrefabs;
            default: return mathPrefabs;
        }
    }

    GameObject FindPrefabByName(GameObject[] prefabs, string name)
    {
        if (prefabs == null || string.IsNullOrEmpty(name)) return null;
        foreach (var p in prefabs)
        {
            if (p != null && p.name.Equals(name, System.StringComparison.OrdinalIgnoreCase))
                return p;
        }
        return null;
    }
}
