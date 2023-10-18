using UnityEngine;
using TMPro;

public class SettingsUIController : MonoBehaviour
{
    private static SettingsUIController singleton;

    [Header("Player Preferences")]

    [SerializeField] TMP_Dropdown screenModeDropdown;

    bool InitializeSingleton()
    {
        if (singleton != null)
        {
            Debug.LogWarning("Settings UI already exists in the scene!");
            return false;
        }

        singleton = this;

        return true;
    }

    public void OpenSettingsMenu()
    {
        if (!InitializeSingleton()) return;

        Instantiate(this);
    }

    public void UpdateFullScreenMode()
    {
        Screen.fullScreenMode = (FullScreenMode)screenModeDropdown.value;
        Debug.Log($"Screen Mode set to: {(FullScreenMode)screenModeDropdown.value}");
    }

    private void OnDestroy()
    {
        singleton = null;
    }
}