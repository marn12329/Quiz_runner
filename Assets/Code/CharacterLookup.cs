using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class CharacterLookup
{
    private static Dictionary<string, Sprite> nameToSprite = new Dictionary<string, Sprite>();
    private static bool initialized = false;

    public static void InitializeAll()
    {
        if (initialized) return;
        initialized = true;

        LoadAllSprites("Characters/Math");
        LoadAllSprites("Characters/Thai");
        LoadAllSprites("Characters/English");
        LoadAllSprites("Characters"); // fallback รวมทุกอัน

        Debug.Log($"✅ CharacterLookup initialized with {nameToSprite.Count} sprites");
    }

    private static void LoadAllSprites(string path)
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>(path);
        foreach (var s in sprites)
        {
            if (s == null) continue;
            if (!nameToSprite.ContainsKey(s.name))
            {
                nameToSprite[s.name] = s;
                Debug.Log($"[CharacterLookup] Registered sprite for '{s.name}' from {path}");
            }
        }
    }

    public static void RegisterPrefab(GameObject prefab)
    {
        if (prefab == null) return;
        string key = prefab.name;
        if (nameToSprite.ContainsKey(key)) return;

        Sprite found = null;
        SpriteRenderer sr = prefab.GetComponentInChildren<SpriteRenderer>(true);
        if (sr != null && sr.sprite != null) found = sr.sprite;

        if (found == null)
        {
            Image img = prefab.GetComponentInChildren<Image>(true);
            if (img != null && img.sprite != null) found = img.sprite;
        }

        if (found != null)
        {
            nameToSprite[key] = found;
            Debug.Log($"[CharacterLookup] Registered sprite from prefab '{key}'");
        }
    }

    public static Sprite TryLoadFromResources(string resourceName)
    {
        if (string.IsNullOrEmpty(resourceName)) return null;
        Sprite s = Resources.Load<Sprite>($"Characters/{resourceName}");
        if (s == null) s = Resources.Load<Sprite>(resourceName);
        if (s != null)
        {
            nameToSprite[resourceName] = s;
            Debug.Log($"[CharacterLookup] Loaded sprite from Resources '{resourceName}'");
        }
        return s;
    }

    public static Sprite GetSprite(string name)
    {
        InitializeAll();
        if (string.IsNullOrEmpty(name)) return null;
        if (nameToSprite.TryGetValue(name, out Sprite s)) return s;
        return TryLoadFromResources(name);
    }
}
