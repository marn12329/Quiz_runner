using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Player Settings")]
    public Transform[] spawnPoints;          // จุด Spawn ของผู้เล่น
    public GameObject[] characterPrefabs;    // Prefab ตัวละครทั้งหมด (ชื่อใน Inspector ต้องตรงกับที่ Save ใน PlayerPrefs)

    void Start()
    {
        SpawnPlayers();
    }

    private void SpawnPlayers()
    {
        if (spawnPoints.Length < 4)
        {
            Debug.LogError("Spawn Points ต้องมีอย่างน้อย 4 จุด!");
            return;
        }

        for (int i = 0; i < 4; i++)
        {
            // อ่านชื่อ Prefab จาก PlayerPrefs
            string characterName = PlayerPrefs.GetString($"Player{i}_Character", "");
            if (string.IsNullOrEmpty(characterName))
            {
                Debug.LogWarning($"Player {i + 1} ไม่มีตัวละครที่เลือก ใช้ตัวเริ่มต้นแทน");
                characterName = characterPrefabs[0].name; // ใช้ตัวแรกเป็น Default
            }

            // หาตัว Prefab ที่ตรงกับชื่อ
            GameObject prefab = FindCharacterPrefab(characterName);
            if (prefab == null)
            {
                Debug.LogError($"ไม่พบ Prefab: {characterName}");
                continue;
            }

            // สร้างตัวละคร
            Transform spawnPoint = spawnPoints[i];
            GameObject player = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);

            // ตั้งค่า PlayerIndex ให้ตัวละคร
            PlayerIndex index = player.GetComponent<PlayerIndex>();
            if (index == null)
            {
                index = player.AddComponent<PlayerIndex>();
            }
            index.SetPlayerID(i);

            // ตั้งชื่อผู้เล่นจาก PlayerData
            if (PlayerData.playerNames != null && i < PlayerData.playerNames.Length)
            {
                PlayerMovement movement = player.GetComponent<PlayerMovement>();
                if (movement != null)
                {
                    movement.SetPlayerName(PlayerData.playerNames[i]);
                }
            }
        }
    }

    private GameObject FindCharacterPrefab(string name)
    {
        foreach (var prefab in characterPrefabs)
        {
            if (prefab.name == name)
                return prefab;
        }
        return null;
    }
}
