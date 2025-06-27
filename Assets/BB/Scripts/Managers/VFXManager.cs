using UnityEngine;
public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance { get; private set; }
    public GameObject bubblePopEffect;
    public GameObject plantGrowEffect;
    void Awake() { if (Instance != null) Destroy(gameObject); else { Instance = this; DontDestroyOnLoad(gameObject); } }
    public void PlayBubblePopVFX(Vector3 position) 
    {
        if (bubblePopEffect == null) return; 
        GameObject effectInstance = Instantiate(bubblePopEffect, position, Quaternion.identity);
        Destroy(effectInstance, 1f);
    }

    public void PlayPlantGrowVFX(Vector3 position) 
    {
        if (plantGrowEffect == null) return;
        GameObject effectInstance = Instantiate(plantGrowEffect, position, Quaternion.identity);
        // Get the ParticleSystem component
        ParticleSystem ps = effectInstance.GetComponent<ParticleSystem>();
        Destroy(effectInstance, 1f);
    }

}