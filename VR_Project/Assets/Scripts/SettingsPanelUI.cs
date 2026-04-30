using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsPanelUI : MonoBehaviour
{
    public GameObject mainSettingsView;
    public GameObject volumeView;
    public GameObject controlsView;

    public void Resume()
    {
        gameObject.SetActive(false);
    }

    public void ShowVolume()
    {
        mainSettingsView.SetActive(false);
        volumeView.SetActive(true);
        controlsView.SetActive(false);
    }

    public void ShowControls()
    {
        mainSettingsView.SetActive(false);
        volumeView.SetActive(false);
        controlsView.SetActive(true);
    }

    public void BackToSettings()
    {
        mainSettingsView.SetActive(true);
        volumeView.SetActive(false);
        controlsView.SetActive(false);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}