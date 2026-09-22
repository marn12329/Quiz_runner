using UnityEngine;
using TMPro;

public class PlayerNameFollow : MonoBehaviour
{
    public Transform player;             // ตัวละครที่ชื่อจะตาม
    public Vector3 offset = new Vector3(0, 2f, 0); // ระยะห่างเหนือหัว
    private Camera mainCamera;           
    private TextMeshProUGUI nameText;    

    public string playerName = "Player"; // ชื่อผู้เล่น

    void Start()
    {
        nameText = GetComponent<TextMeshProUGUI>();
        mainCamera = Camera.main;

        if (nameText != null)
            nameText.text = playerName;
    }

    void Update()
    {
        if (player != null && mainCamera != null)
        {
            // แปลงตำแหน่งโลกของผู้เล่นเป็นตำแหน่งจอ
            Vector3 screenPos = mainCamera.WorldToScreenPoint(player.position + offset);
            transform.position = screenPos;
        }
    }
}
