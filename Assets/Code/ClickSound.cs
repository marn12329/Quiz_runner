using UnityEngine;

public class ClickSound : MonoBehaviour
{
    public AudioClip clickClip;

    public void PlayClick()
    {
        SFXAudioPlayer.Instance.PlaySFX(clickClip, false);
    }
}
