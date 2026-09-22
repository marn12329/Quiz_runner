using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerIndex))]
public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    public int playerID = -1;
    public int quadrantID = -1;
    public string playerName = "Player";
    public float baseSpeed = 3f;
    public float runDistance = 3f;

    private Vector3 targetPosition;
    private bool isRunning = false;

    private PlayerIndex playerIndex;
    private bool isFrozen = false;
    private bool hasShield = false;
    private float currentSpeed;

    private Coroutine speedBoostCoroutine;
    private Coroutine shieldCoroutine;

    void Awake()
    {
        playerIndex = GetComponent<PlayerIndex>();
        if (playerIndex != null)
            playerID = playerIndex.playerID;
    }

    void Start()
    {
        playerName = PlayerData.GetPlayerName(playerID);
        targetPosition = transform.position;
        currentSpeed = baseSpeed;

        Invoke(nameof(UpdateNameUI), 0.2f);
    }

    void Update()
    {
        // ตรวจสอบสถานะแช่แข็งจาก PlayerData
        if (PlayerData.freezeTurns[playerID] > 0)
            isFrozen = true;
        else if (isFrozen)
            isFrozen = false;

        // การเคลื่อนที่
        if (isRunning && !isFrozen)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, currentSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
                isRunning = false;
        }
    }

    // -------------------------------------------------
    // การเคลื่อนไหว
    // -------------------------------------------------
    public void StartRunning()
    {
        if (isFrozen) return;
        if (!isRunning)
        {
            targetPosition = transform.position + Vector3.right * runDistance;
            isRunning = true;
        }
    }

    // -------------------------------------------------
    // เอฟเฟกต์ Freeze / Shield
    // -------------------------------------------------
    public void SetFrozen(bool state)
    {
        if (hasShield && state)
        {
            Debug.Log($"🛡 Player {playerID} shielded from freeze!");
            return;
        }

        isFrozen = state;
        Debug.Log(state ? $"🧊 Player {playerID} frozen!" : $"🔥 Player {playerID} unfrozen!");
    }

    public bool IsFrozen() => isFrozen;

    public static void FreezeOthers(int selfID, int turns = 1)
    {
        for (int i = 0; i < 4; i++)
        {
            if (i == selfID) continue;
            PlayerData.ApplyFreezeToPlayer(i, turns);
            Debug.Log($"❄ Player {i} frozen for {turns} turn(s) by Player {selfID}");
        }
    }

    public static void SkipTurn(int targetID)
    {
        PlayerData.SetSkipNextTurn(targetID, true);
        Debug.Log($"⏭ Player {targetID} will skip next turn!");
    }

    // -------------------------------------------------
    // Speed Boost
    // -------------------------------------------------
    public void ApplySpeedMultiplier(float multiplier)
    {
        if (speedBoostCoroutine != null)
            StopCoroutine(speedBoostCoroutine);

        speedBoostCoroutine = StartCoroutine(SpeedBoostTemp(multiplier, 5f));
    }

    private IEnumerator SpeedBoostTemp(float multiplier, float duration)
    {
        currentSpeed = baseSpeed * multiplier;
        Debug.Log($"⚡ Player {playerID} boosted x{multiplier}!");
        yield return new WaitForSeconds(duration);
        ResetSpeed();
    }

    public void ResetSpeed()
    {
        currentSpeed = baseSpeed;
        Debug.Log($"🏃 Player {playerID} speed reset.");
    }

    // -------------------------------------------------
    // Shield
    // -------------------------------------------------
    public void SetShield(float duration)
    {
        if (shieldCoroutine != null)
            StopCoroutine(shieldCoroutine);

        shieldCoroutine = StartCoroutine(ShieldTemp(duration));
    }

    private IEnumerator ShieldTemp(float duration)
    {
        hasShield = true;
        Debug.Log($"🛡 Player {playerID} gained shield for {duration}s!");
        yield return new WaitForSeconds(duration);
        hasShield = false;
        Debug.Log($"🛡 Player {playerID}'s shield expired.");
    }

    public bool HasShield() => hasShield;

    // -------------------------------------------------
    // รีเซ็ตระบบชั่วคราวของผู้เล่นคนเดียว
    // -------------------------------------------------
    public void ResetTemporaryEffects()
    {
        isFrozen = false;
        hasShield = false;
        currentSpeed = baseSpeed;

        if (speedBoostCoroutine != null) StopCoroutine(speedBoostCoroutine);
        if (shieldCoroutine != null) StopCoroutine(shieldCoroutine);

        speedBoostCoroutine = null;
        shieldCoroutine = null;

        PlayerData.freezeTurns[playerID] = 0;
        PlayerData.slowTurns[playerID] = 0;
        PlayerData.doubleRunCounts[playerID] = 0;
        PlayerData.skipNextTurn[playerID] = false;

        Debug.Log($"♻ Player {playerID} reset all temporary effects.");
    }

    // -------------------------------------------------
    // รีเซ็ตทุกคน (ตอนตอบผิดหรือได้ไอเท็มใหม่)
    // -------------------------------------------------
    public static void ClearAllForNewState()
    {
        PlayerController[] allPlayers = FindObjectsOfType<PlayerController>();
        foreach (var p in allPlayers)
        {
            p.ResetTemporaryEffects();
        }

        PlayerData.ResetAllStatuses();
        Debug.Log("🔄 All players cleared (from new item or wrong answer).");
    }

    // -------------------------------------------------
    // UI
    // -------------------------------------------------
    private void UpdateNameUI()
    {
        PlayerNameUI ui = GetComponent<PlayerNameUI>();
        if (ui != null)
            ui.SendMessage("UpdatePlayerName", SendMessageOptions.DontRequireReceiver);
    }
}
