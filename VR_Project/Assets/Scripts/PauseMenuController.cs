using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenuController : MonoBehaviour
{
    [Header("Settings / Pause Menu")]
    public GameObject settingsPanel;
    public GameObject mainSettingsView;
    public GameObject volumeView;
    public GameObject controlsView;

    [Header("Input Action")]
    public InputActionReference menuButtonAction;

    private bool isPaused = false;
    private float lastToggleTime = -1f;
    private float toggleCooldown = 0.25f;

    private void Start()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        Time.timeScale = 1f;
        Debug.Log("PauseMenuController started. Time Scale: " + Time.timeScale);
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.mKey.wasPressedThisFrame)
        {
            TogglePauseMenu();
        }
    }

    private void OnEnable()
    {
        if (menuButtonAction != null)
        {
            menuButtonAction.action.performed += OnMenuButtonPressed;
            menuButtonAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (menuButtonAction != null)
        {
            menuButtonAction.action.performed -= OnMenuButtonPressed;
            menuButtonAction.action.Disable();
        }
    }

    private void OnMenuButtonPressed(InputAction.CallbackContext context)
    {
        TogglePauseMenu();
    }

    public void TogglePauseMenu()
    {
        if (Time.unscaledTime - lastToggleTime < toggleCooldown)
            return;

        lastToggleTime = Time.unscaledTime;

        if (isPaused)
            ClosePauseMenu();
        else
            OpenPauseMenu();
    }

    public void OpenPauseMenu()
    {
        isPaused = true;

        if (settingsPanel != null)
            settingsPanel.SetActive(true);

        if (mainSettingsView != null)
            mainSettingsView.SetActive(true);

        if (volumeView != null)
            volumeView.SetActive(false);

        if (controlsView != null)
            controlsView.SetActive(false);

        Time.timeScale = 0f;

        Debug.Log("GAME PAUSED. Time Scale: " + Time.timeScale);
    }

    public void ClosePauseMenu()
    {
        isPaused = false;

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        Time.timeScale = 1f;

        Debug.Log("GAME RESUMED. Time Scale: " + Time.timeScale);
    }
}