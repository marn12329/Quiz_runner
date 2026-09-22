using UnityEngine;
using UnityEngine.UI;

public class ButtonMashUI : MonoBehaviour
{
    public Image[] playerIndicators;  // UI ของผู้เล่นแต่ละคน
    public KeyCode[] playerKeys = { KeyCode.A, KeyCode.S, KeyCode.K, KeyCode.L }; // ปุ่มของผู้เล่น
    public ButtonMashGameManager gameManager; // อ้างอิงไปที่ GameManager

    private Color defaultColor = Color.white;
    private Color pressColor = Color.yellow;

    void Update()
    {
        for (int i = 0; i < playerKeys.Length; i++)
        {
            if (Input.GetKeyDown(playerKeys[i]))
            {
                // ถ้าเกมกำลังรัน → ให้บวกคะแนนและกระพิบ
                if (gameManager != null && gameManager.IsGameRunning())
                {
                    gameManager.AddScore(i);
                    StartCoroutine(FlashIndicator(playerIndicators[i]));
                }
            }
        }
    }

    System.Collections.IEnumerator FlashIndicator(Image indicator)
    {
        indicator.color = pressColor;
        yield return new WaitForSeconds(0.1f);
        indicator.color = defaultColor;
    }
}
