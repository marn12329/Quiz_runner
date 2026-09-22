using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelectScene : MonoBehaviour
{
    // ถ้าต้องการ ให้เรียกเซฟชื่อด้วย (ตัวอย่างนี้เซฟ player 0)
    // คุณอาจจะขยายเป็นหลายผู้เล่นได้ตามต้องการ

    // เรียกจากปุ่ม UI: OnClick -> CharacterSelectScene.OnCharacterSelected("HeroA")
    public void OnCharacterSelected(string charName)
    {
        // บันทึกตัวละครสำหรับ Player 0 (ถ้าระบบหลายผู้เล่นก็ปรับ id ให้ถูก)
        int playerId = 0;

        // บันทึกตัวละคร (แยกตาม Subject)
        PlayerData.SetPlayerCharacter(playerId, charName);

        // (ถ้าต้องการให้ผู้เล่นใส่ชื่อด้วย ให้เรียก Scene ที่มี UI ใส่ชื่อก่อน)
        // ตัวอย่างนี้ไปมินิเกมเลย:
        if (SceneFlowManager.Instance != null && !string.IsNullOrEmpty(SceneFlowManager.Instance.selectedMiniGameScene))
        {
            SceneManager.LoadScene(SceneFlowManager.Instance.selectedMiniGameScene);
        }
        else
        {
            Debug.LogWarning("No mini game scene set in SceneFlowManager. Loading default MiniGameScene.");
            SceneManager.LoadScene("MiniGameScene");
        }
    }
}
