using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class OnScreenKeyboardUI : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField inputField;

    [Header("Letter Buttons (A–Z)")]
    public Button[] letterButtons;

    [Header("Special Buttons")]
    public Button spaceButton;
    public Button backspaceButton;
    public Button okButton;

    private string currentText = "";

    void Start()
    {
        if (inputField == null)
        {
            Debug.LogError("❌ InputField ยังไม่ได้ Assign!");
            return;
        }

        // ✅ ผูกปุ่มตัวอักษร A–Z
        for (int i = 0; i < letterButtons.Length; i++)
        {
            int index = i; // ป้องกันปัญหา closure
            if (letterButtons[i] != null)
            {
                string letter = ((char)('A' + index)).ToString();
                letterButtons[i].onClick.AddListener(() => AddLetter(letter));
            }
        }

        // ✅ ปุ่มพิเศษ
        if (spaceButton != null)
            spaceButton.onClick.AddListener(() => AddLetter(" "));

        if (backspaceButton != null)
            backspaceButton.onClick.AddListener(Backspace);

        if (okButton != null)
            okButton.onClick.AddListener(Confirm);
    }

    private void AddLetter(string letter)
    {
        currentText += letter;
        inputField.text = currentText;
    }

    private void Backspace()
    {
        if (currentText.Length > 0)
        {
            currentText = currentText.Substring(0, currentText.Length - 1);
            inputField.text = currentText;
        }
    }

    private void Confirm()
    {
        Debug.Log($"✅ พิมพ์ชื่อเสร็จ: {currentText}");
        // ซ่อนคีย์บอร์ดหลังพิมพ์เสร็จ
        gameObject.SetActive(false);
    }
}
