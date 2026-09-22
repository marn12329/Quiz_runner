using UnityEngine;

public static class PlayerData
{
    public enum SubjectType { None, Math, Thai, English }
    public static SubjectType CurrentSubject = SubjectType.None;

    // -------------------------------
    // SUBJECT
    // -------------------------------
    public static void SetSubject(SubjectType subject)
    {
        CurrentSubject = subject;
        PlayerPrefs.SetString("CurrentSubject", subject.ToString());
        PlayerPrefs.Save();
        Debug.Log($"📘 Selected subject: {subject}");
    }

    public static void SetSubject(string subjectStr)
    {
        if (System.Enum.TryParse(subjectStr, out SubjectType s))
            SetSubject(s);
        else
            SetSubject(SubjectType.None);
    }

    public static void LoadLastSubject()
    {
        string s = PlayerPrefs.GetString("CurrentSubject", "None");
        if (System.Enum.TryParse(s, out SubjectType subject))
            CurrentSubject = subject;
        else
            CurrentSubject = SubjectType.None;
    }

    public static string GetSubject() => CurrentSubject.ToString();

    // -------------------------------
    // PLAYER DATA
    // -------------------------------
    public static string[] playerNames = new string[4];
    public static string[] playerCharacters = new string[4];
    public static int[] freezeTurns = new int[4];
    public static int[] slowTurns = new int[4];
    public static int[] doubleRunCounts = new int[4];
    public static bool[] skipNextTurn = new bool[4];

    private static string SubjectPrefix =>
        CurrentSubject == SubjectType.None ? "Global" : CurrentSubject.ToString();

    public static bool IsValidPlayerID(int id)
    {
        if (id < 0 || id >= playerNames.Length)
        {
            Debug.LogWarning($"⚠ Invalid Player ID: {id}");
            return false;
        }
        return true;
    }

    // -------------------------------
    // PLAYER NAME
    // -------------------------------
    public static void SetPlayerName(int id, string name)
    {
        if (!IsValidPlayerID(id)) return;
        playerNames[id] = name;

        string globalKey = $"Global_Player{id + 1}_Name";
        PlayerPrefs.SetString(globalKey, name);

        if (CurrentSubject != SubjectType.None)
        {
            string key = $"{SubjectPrefix}_Player{id + 1}_Name";
            PlayerPrefs.SetString(key, name);
        }

        PlayerPrefs.Save();
        Debug.Log($"💾 Saved name (id={id}): '{name}' [Global + {SubjectPrefix}]");
    }

    public static string GetPlayerName(int id)
    {
        if (!IsValidPlayerID(id)) return $"Player {id + 1}";

        string subjectKey = $"{SubjectPrefix}_Player{id + 1}_Name";
        string globalKey = $"Global_Player{id + 1}_Name";

        string name = PlayerPrefs.GetString(subjectKey, "");
        if (string.IsNullOrEmpty(name))
            name = PlayerPrefs.GetString(globalKey, $"Player {id + 1}");

        playerNames[id] = name;
        return name;
    }

    // -------------------------------
    // CHARACTER
    // -------------------------------
    public static void SetPlayerCharacter(int id, string characterName)
    {
        if (!IsValidPlayerID(id)) return;
        playerCharacters[id] = characterName;

        string globalKey = $"Global_Player{id + 1}_Character";
        PlayerPrefs.SetString(globalKey, characterName);

        if (CurrentSubject != SubjectType.None)
        {
            string key = $"{SubjectPrefix}_Player{id + 1}_Character";
            PlayerPrefs.SetString(key, characterName);
        }

        PlayerPrefs.Save();
        Debug.Log($"🎭 Saved character (id={id}): '{characterName}' [Global + {SubjectPrefix}]");
    }

    public static string GetPlayerCharacter(int id)
    {
        if (!IsValidPlayerID(id)) return "DefaultCharacter";

        string subjectKey = $"{SubjectPrefix}_Player{id + 1}_Character";
        string globalKey = $"Global_Player{id + 1}_Character";

        string chr = PlayerPrefs.GetString(subjectKey, "");
        if (string.IsNullOrEmpty(chr))
            chr = PlayerPrefs.GetString(globalKey, "DefaultCharacter");

        playerCharacters[id] = chr;
        return chr;
    }

    public static string GetCharacterName(int id) => GetPlayerCharacter(id);

    // -------------------------------
    // LOAD ALL
    // -------------------------------
    public static void LoadAllNames()
    {
        LoadLastSubject();

        for (int i = 0; i < playerNames.Length; i++)
        {
            playerNames[i] = GetPlayerName(i);
            playerCharacters[i] = GetPlayerCharacter(i);
        }

        Debug.Log($"📂 Loaded all player names & characters for subject: {CurrentSubject}");
    }

    // -------------------------------
    // EFFECT SYSTEM
    // -------------------------------
    public static void ApplyFreezeToPlayer(int targetID, int turns = 1)
    {
        if (!IsValidPlayerID(targetID)) return;
        freezeTurns[targetID] = turns;
        Debug.Log($"🧊 Freeze Player {targetID} for {turns} turn(s)");
    }

    public static void ApplySlowToPlayer(int targetID, int turns = 1)
    {
        if (!IsValidPlayerID(targetID)) return;
        slowTurns[targetID] = turns;
        Debug.Log($"🐌 Slow Player {targetID} for {turns} turn(s)");
    }

    public static void AddDoubleRun(int id, int count = 1)
    {
        if (!IsValidPlayerID(id)) return;
        doubleRunCounts[id] += count;
        Debug.Log($"🏃‍♂️ Player {id} gained double-run ({doubleRunCounts[id]} total)");
    }

    public static void SetSkipNextTurn(int id, bool skip)
    {
        if (!IsValidPlayerID(id)) return;
        skipNextTurn[id] = skip;
        if (skip)
            Debug.Log($"⏭ Player {id} will skip next turn!");
    }

    // -------------------------------
    // CLEAR STATUS (PER PLAYER)
    // -------------------------------
    public static void ClearPlayerStatuses(int id)
    {
        if (!IsValidPlayerID(id)) return;

        freezeTurns[id] = 0;
        slowTurns[id] = 0;
        doubleRunCounts[id] = 0;
        skipNextTurn[id] = false;

        Debug.Log($"♻ Cleared all statuses for Player {id}");
    }

    // -------------------------------
    // CLEAR ALL (ใช้ตอนตอบผิด / ได้ไอเท็มใหม่)
    // -------------------------------
    public static void ResetAllStatuses()
    {
        for (int i = 0; i < playerNames.Length; i++)
        {
            ClearPlayerStatuses(i);
        }
        Debug.Log($"🧼 Cleared all statuses for ALL players ({SubjectPrefix})");
    }

    // -------------------------------
    // TICK & RESET END ROUND
    // -------------------------------
    public static void TickEndOfRound()
    {
        for (int i = 0; i < playerNames.Length; i++)
        {
            if (freezeTurns[i] > 0) freezeTurns[i]--;
            if (slowTurns[i] > 0) slowTurns[i]--;
        }
        Debug.Log($"⏱ End of round tick complete for: {SubjectPrefix}");
    }
}
