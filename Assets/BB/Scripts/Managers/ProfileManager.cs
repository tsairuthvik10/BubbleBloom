using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ProfileManager : MonoBehaviour
{
    public static ProfileManager Instance { get; private set; }

    [Header("UI Elements")]
    public GameObject profileCreationPanel;
    public TMP_InputField nameInputField;
    public Button saveButton;

    private const string PLAYER_NAME_KEY = "PlayerName";
    public string PlayerName { get; private set; }

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        PlayerName = PlayerPrefs.GetString(PLAYER_NAME_KEY, "");
        if (string.IsNullOrEmpty(PlayerName))
        {
            if (profileCreationPanel != null) profileCreationPanel.SetActive(true);
        }
        else
        {
            if (profileCreationPanel != null) profileCreationPanel.SetActive(false);
        }
    }

    public void SaveProfile()
    {
        if (string.IsNullOrWhiteSpace(nameInputField.text)) return;

        PlayerName = nameInputField.text;
        PlayerPrefs.SetString(PLAYER_NAME_KEY, PlayerName);
        PlayerPrefs.Save();

        if (profileCreationPanel != null) profileCreationPanel.SetActive(false);
    }
}
