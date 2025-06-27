using UnityEngine;
public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance { get; private set; }
    public GameObject bubblePopEffect;
    public GameObject plantGrowEffect;
    void Awake() { if (Instance != null) Destroy(gameObject); else { Instance = this; DontDestroyOnLoad(gameObject); } }
    public void PlayBubblePopVFX(Vector3 position) { if (bubblePopEffect != null) Instantiate(bubblePopEffect, position, Quaternion.identity); }
    public void PlayPlantGrowVFX(Vector3 position) { if (plantGrowEffect != null) Instantiate(plantGrowEffect, position, Quaternion.identity); }
}