using UnityEngine;

public class SFXAudioPlayer : MonoBehaviour
{
    public static SFXAudioPlayer Instance;

    public AudioSource sfxSource;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    /// <summary>
    /// เล่นเสียง SFX (เลือกว่าจะลูปหรือไม่)
    /// </summary>
    public void PlaySFX(AudioClip clip, bool loop = false)
    {
        if (sfxSource == null || clip == null) return;

        sfxSource.loop = loop;
        sfxSource.clip = clip;
        sfxSource.Play();
    }

    public void StopSFX()
    {
        if (sfxSource != null)
            sfxSource.Stop();
    }
}
