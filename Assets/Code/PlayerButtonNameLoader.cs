// PlayerButtonNameLoader.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class PlayerButtonNameLoader : MonoBehaviour
{
    [Tooltip("Player index (0..3) that this button represents")]
    public int playerIndex = 0;

    [Tooltip("ถ้าใช้ TextMeshPro แบบ child ให้ลาก TextMeshProUGUI, ถ้าใช้ Text ให้ลาก Text (optional)")]
    public TextMeshProUGUI tmpText;
    public Text legacyText;

    void Start()
    {
        UpdateName();
    }

    public void UpdateName()
    {
        string name = PlayerData.GetPlayerName(playerIndex);
        if (tmpText != null) tmpText.text = name;
        if (legacyText != null) legacyText.text = name;
    }
}
