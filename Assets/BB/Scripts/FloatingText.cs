using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    public float floatSpeed = 1.0f;
    public float fadeOutTime = 1.0f;
    private TextMeshProUGUI textMesh;
    private float initialFadeTime;

    void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        initialFadeTime = fadeOutTime;
    }

    void Update()
    {
        transform.position += new Vector3(0, floatSpeed * Time.deltaTime, 0);
        fadeOutTime -= Time.deltaTime;
        if (fadeOutTime > 0)
        {
            Color newColor = textMesh.color;
            newColor.a = fadeOutTime / initialFadeTime;
            textMesh.color = newColor;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}