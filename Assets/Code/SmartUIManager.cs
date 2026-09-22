using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

[DefaultExecutionOrder(-100)]
public class SmartUIManager : MonoBehaviour
{
    [Header("📱 Canvas ปรับอัตโนมัติ")]
    public CanvasScaler[] allCanvas;

    [Header("🔠 Font Scale")]
    public float phoneFontScale = 1.0f;
    public float tabletFontScale = 1.3f;
    public float tvFontScale = 1.6f;
    public float pcFontScale = 1.0f;

    [Header("📏 Layout Scale (ปุ่ม, ช่อง Grid ฯลฯ)")]
    public float phoneLayoutScale = 1.0f;
    public float tabletLayoutScale = 1.3f;
    public float tvLayoutScale = 1.6f;
    public float pcLayoutScale = 1.0f;

    private static SmartUIManager instance;
    private float currentFontScale = 1f;
    private float currentLayoutScale = 1f;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += (scene, mode) => AdjustUI();

        AdjustUI();
    }

    void Start()
    {
        if (allCanvas == null || allCanvas.Length == 0)
            allCanvas = FindObjectsOfType<CanvasScaler>();
    }

    void AdjustUI()
    {
        if (allCanvas == null || allCanvas.Length == 0)
            allCanvas = FindObjectsOfType<CanvasScaler>();

        float aspect = (float)Screen.width / Screen.height;
        float fontScale, layoutScale;

#if UNITY_ANDROID || UNITY_IOS
        if (Screen.width <= 1280)
        {
            fontScale = phoneFontScale;
            layoutScale = phoneLayoutScale;
        }
        else
        {
            fontScale = tabletFontScale;
            layoutScale = tabletLayoutScale;
        }
#elif UNITY_TVOS || UNITY_STANDALONE
        fontScale = tvFontScale;
        layoutScale = tvLayoutScale;
#else
        fontScale = pcFontScale;
        layoutScale = pcLayoutScale;
#endif

        currentFontScale = fontScale;
        currentLayoutScale = layoutScale;

        foreach (CanvasScaler scaler in allCanvas)
        {
            if (scaler == null) continue;
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = aspect > 1.6f ? 1 : 0;
        }

        AdjustFonts(fontScale);
        AdjustLayouts(layoutScale);

        Debug.Log($"✅ SmartUIManager ปรับ UI เรียบร้อย (Font x{fontScale}, Layout x{layoutScale})");
    }

    void AdjustFonts(float scale)
    {
        foreach (TMP_Text text in FindObjectsOfType<TMP_Text>())
        {
            text.fontSize *= scale;
        }
    }

    void AdjustLayouts(float scale)
    {
        foreach (GridLayoutGroup grid in FindObjectsOfType<GridLayoutGroup>())
        {
            grid.cellSize *= scale;
            grid.spacing *= scale;
        }

        foreach (HorizontalOrVerticalLayoutGroup layout in FindObjectsOfType<HorizontalOrVerticalLayoutGroup>())
        {
            layout.spacing *= scale;
            layout.padding.left = Mathf.RoundToInt(layout.padding.left * scale);
            layout.padding.right = Mathf.RoundToInt(layout.padding.right * scale);
            layout.padding.top = Mathf.RoundToInt(layout.padding.top * scale);
            layout.padding.bottom = Mathf.RoundToInt(layout.padding.bottom * scale);
        }
    }

    public void ForceUpdateUI()
    {
        AdjustUI();
    }
}
