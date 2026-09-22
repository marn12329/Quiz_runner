using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUIController : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject menuPanel;

    private bool isOpen = false;

    void Start()
    {
        if (menuPanel != null)
            menuPanel.SetActive(false);
    }

    public void ToggleMenu()
    {
        isOpen = !isOpen;
        menuPanel.SetActive(isOpen);
        Time.timeScale = isOpen ? 0f : 1f;
    }

    // 🔁 รีเกม + ดีเลย์
    public void RestartGame()
    {
        Time.timeScale = 1f;
        StartCoroutine(RestartDelay());
    }

    private System.Collections.IEnumerator RestartDelay()
    {
        // 🕓 ดีเลย์ (ปรับค่าได้)
        yield return new WaitForSeconds(0.5f);

        // ❗ ลบ SceneFlowManager ตัวเก่า (ป้องกันซ้ำ)
        if (SceneFlowManager.Instance != null)
        {
            Destroy(SceneFlowManager.Instance.gameObject);
        }

        // 🔄 โหลดซีนแรกจาก Build Settings
        SceneManager.LoadScene(0);
    }

    public void CloseMenu()
    {
        isOpen = false;
        menuPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}
