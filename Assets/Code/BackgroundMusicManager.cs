using UnityEngine;

public class BackgroundMusicManager : MonoBehaviour
{
    public static BackgroundMusicManager Instance;

    [Header("Background Music Source")]
    public AudioSource bgmSource;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (bgmSource != null)
            bgmSource.loop = true; // ค่า default = ลูป
    }

    /// <summary>
    /// เล่นเพลง BGM (เลือกได้ว่าจะลูปหรือไม่ลูป)
    /// </summary>
    public void PlayBGM(AudioClip clip, bool loop = true)
    {
        if (bgmSource == null || clip == null) return;

        bgmSource.loop = loop;
        bgmSource.clip = clip;
        bgmSource.Play();
    }

    /// <summary>
    /// ปิดเพลง BGM
    /// </summary>
    public void StopBGM()
    {
        if (bgmSource != null)
            bgmSource.Stop();
    }
}
