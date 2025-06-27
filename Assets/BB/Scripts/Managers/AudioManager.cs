using UnityEngine;
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    public AudioSource musicSource, sfxSource;
    public AudioClip bubblePopSound;
    public AudioClip plantGrowSound;
    void Awake() { if (Instance != null) Destroy(gameObject); else { Instance = this; DontDestroyOnLoad(gameObject); } }
    public void PlayBubblePop() { if (bubblePopSound != null) sfxSource.PlayOneShot(bubblePopSound); }
    public void PlayPlantGrow() { if (plantGrowSound != null) sfxSource.PlayOneShot(plantGrowSound); }
}
