using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUIManager : MonoBehaviour
{
    [Header("เมนู UI Panel")]
    public GameObject menuPanel;     // UI เมนูหลัก
    public GameObject settingsPanel; // ถ้ามีหน้า Setting เพิ่ม
    public GameObject exitPanel;     // ถ้ามีหน้า Exit ยืนยัน

    void Start()
    {
        // ปิดเมนูทั้งหมดตอนเริ่มเกม
        if (menuPanel) menuPanel.SetActive(false);
        if (settingsPanel) settingsPanel.SetActive(false);
        if (exitPanel) exitPanel.SetActive(false);
    }

    // -------------------------------
    // เปิดเมนู
    // -------------------------------
    public void ToggleMenu()
    {
        bool newState = !menuPanel.activeSelf;
        menuPanel.SetActive(newState);

        // หยุดเกมตอนเปิดเมนู (ถ้าต้องการ)
        Time.timeScale = newState ? 0f : 1f;
    }

    // -------------------------------
    // ปุ่มในเมนู
    // -------------------------------
    public void OpenSettings()
    {
        if (settingsPanel)
        {
            settingsPanel.SetActive(true);
            menuPanel.SetActive(false);
        }
    }

    public void CloseSettings()
    {
        if (settingsPanel)
        {
            settingsPanel.SetActive(false);
            menuPanel.SetActive(true);
        }
    }

    public void OpenExitConfirm()
    {
        if (exitPanel)
        {
            exitPanel.SetActive(true);
            menuPanel.SetActive(false);
        }
    }

    public void CancelExit()
    {
        if (exitPanel)
        {
            exitPanel.SetActive(false);
            menuPanel.SetActive(true);
        }
    }

    // -------------------------------
    // กลับหน้า Lobby
    // -------------------------------
    public void BackToLobby()
    {
        Time.timeScale = 1f;  
        SceneManager.LoadScene("LobbyScene"); // ใส่ชื่อฉาก Lobby ของคุณ
    }
}
