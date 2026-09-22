using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

[DefaultExecutionOrder(-100)]
public class SmartUIManagerPro : MonoBehaviour
{
    [Header("Canvas Settings")]
    public CanvasScaler[] allCanvas;

    [Header("Font Scale")]
    public float phoneFontScale = 1.0f;
    public float tabletFontScale = 1.3f;
    public float tvFontScale = 1.6f;
    public float pcFontScale = 1.0f;

    [Header("Layout Scale")]
    public float phoneLayoutScale = 1.0f;
    public float tabletLayoutScale = 1.3f;
    public float tvLayoutScale = 1.6f;
    public float pcLayoutScale = 1.0f;

    [Header("RectTransform Scaling")]
    public bool adjustAnchors = true;
    public bool adjustRectSize = true;

    [Header("Safe Area Settings")]
    public bool enableSafeArea = true;
    public RectTransform[] safeAreaRoots;

    private static SmartUIManagerPro instance;
    private float currentFontScale = 1f;
    private float currentLayoutScale = 1f;
    private Rect lastSafeArea = new Rect(0, 0, 0, 0);

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

    void Update()
    {
#if UNITY_ANDROID || UNITY_IOS || UNITY_TVOS
        if (enableSafeArea && Screen.safeArea != lastSafeArea)
        {
            ApplySafeArea();
        }
#endif
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
#elif UNITY_TVOS
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
        AdjustRectTransforms(layoutScale);

        if (enableSafeArea)
            ApplySafeArea();

        Debug.Log($"✅ SmartUIManagerPro: ปรับ UI สำเร็จ (Font x{fontScale}, Layout x{layoutScale})");
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

    void AdjustRectTransforms(float scale)
    {
        if (!adjustAnchors && !adjustRectSize)
            return;

        foreach (RectTransform rect in FindObjectsOfType<RectTransform>())
        {
            if (rect.parent == null) continue; // skip root canvas

            if (adjustAnchors)
            {
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.pivot = new Vector2(0.5f, 0.5f);
            }

            if (adjustRectSize)
            {
                rect.sizeDelta *= scale;
                rect.anchoredPosition *= scale;
            }
        }
    }

    void ApplySafeArea()
    {
#if UNITY_ANDROID || UNITY_IOS || UNITY_TVOS
        Rect safeArea = Screen.safeArea;
        if (safeArea == lastSafeArea) return;
        lastSafeArea = safeArea;

        foreach (RectTransform root in safeAreaRoots)
        {
            if (root == null) continue;

            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = safeArea.position + safeArea.size;

            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            root.anchorMin = anchorMin;
            root.anchorMax = anchorMax;
        }

        Debug.Log($"📱 SafeArea applied: {safeArea}");
#endif
    }

    public void ForceUpdateUI()
    {
        AdjustUI();
    }

#if UNITY_EDITOR
    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.F5))
            ForceUpdateUI();
    }
#endif
}
