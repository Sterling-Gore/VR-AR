using UnityEngine;

public class SettingsViewManager : MonoBehaviour
{
    public GameObject mainSettingsView;
    public GameObject volumeView;
    public GameObject controlsView;

    public void BackToMain()
    {
        mainSettingsView.SetActive(true);
        volumeView.SetActive(false);
        controlsView.SetActive(false);
    }
}