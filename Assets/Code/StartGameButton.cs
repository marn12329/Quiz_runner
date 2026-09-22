using UnityEngine;
using UnityEngine.UI;

public class StartGameButton : MonoBehaviour
{
    public ButtonMashGameManager gameManager;  // อ้างอิงไปที่ GameManager
    private Button startButton;

    void Start()
    {
        // ดึงปุ่มจาก GameObject ปัจจุบัน
        startButton = GetComponent<Button>();

        // ผูก Event ให้ปุ่ม
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartGame);
        }
    }

    void OnStartGame()
    {
        if (gameManager != null)
        {
            gameManager.StartGame(); // เริ่มเกม
            startButton.gameObject.SetActive(false); // ซ่อนปุ่มตอนเริ่มเกม
        }
    }
}
