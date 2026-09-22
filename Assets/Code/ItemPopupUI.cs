using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

public class ItemPopupUI : MonoBehaviour
{
    [Header("UI Components")]
    public GameObject panelRoot;

    [Header("Box Step")]
    public GameObject boxPanel;
    public TextMeshProUGUI titleText;
    public Button acceptButton;
    public Button cancelButton;

    [Header("Result Step")]
    public GameObject resultPanel;
    public TextMeshProUGUI descriptionText;
    public Image itemIcon;
    public TextMeshProUGUI itemNameText;

    private Action onAccept;
    private Action onDecline;

    private void Awake()
    {
        HideInstant();
    }

    /// <summary>
    /// ขั้นตอนที่ 1: ถามว่าจะรับไหม
    /// </summary>
    public void ShowAsk(ItemData item, Action onAccept, Action onDecline)
    {
        this.onAccept = onAccept;
        this.onDecline = onDecline;

        panelRoot.SetActive(true);
        boxPanel.SetActive(true);
        resultPanel.SetActive(false);

        if (titleText != null)
            titleText.text = "จะรับไอเท็มไหม?";

        // ล้าง event เก่า
        acceptButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();

        acceptButton.onClick.AddListener(() =>
        {
            // 🔹 ซ่อนปุ่มทันทีเมื่อกดรับ
            acceptButton.gameObject.SetActive(false);
            cancelButton.gameObject.SetActive(false);
            this.onAccept?.Invoke();
        });

        cancelButton.onClick.AddListener(() =>
        {
            this.onDecline?.Invoke();
        });

        // 🔹 แสดงปุ่มใหม่ทุกครั้งที่ถาม
        acceptButton.gameObject.SetActive(true);
        cancelButton.gameObject.SetActive(true);
    }

    /// <summary>
    /// ขั้นตอนที่ 2: แสดงผลว่าได้รับไอเท็มอะไร
    /// </summary>
    public IEnumerator ShowReceived(ItemData item, float duration)
    {
        boxPanel.SetActive(false);
        resultPanel.SetActive(true);

        if (itemNameText != null)
            itemNameText.text = item.itemName;

        if (descriptionText != null)
        {
            // 🔹 แสดงรายละเอียดของไอเท็มตาม description จริง
            descriptionText.text = $"<b>คำอธิบาย:</b> {item.description}";
        }

        if (itemIcon != null)
            itemIcon.sprite = item.icon;

        yield return new WaitForSecondsRealtime(duration);

        HideInstant();
    }

    public void HideInstant()
    {
        if (panelRoot != null) panelRoot.SetActive(false);
        if (boxPanel != null) boxPanel.SetActive(false);
        if (resultPanel != null) resultPanel.SetActive(false);
    }
}
