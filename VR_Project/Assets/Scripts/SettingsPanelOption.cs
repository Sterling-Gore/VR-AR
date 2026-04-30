using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRSimpleInteractable))]
public class SettingsPanelOption : MonoBehaviour
{
    public Transform selector;
    public TMP_Text optionText;

    public GameObject settingsPanel;
    public GameObject mainSettingsView;
    public GameObject volumeView;
    public GameObject controlsView;

    public string actionName;

    public Color normalColor = Color.white;
    public Color hoverColor = new Color(0.85f, 0.25f, 0.2f);

    private XRSimpleInteractable interactable;
    private bool isHovered = false;

    private void Awake()
    {
        interactable = GetComponent<XRSimpleInteractable>();
    }

    private void OnEnable()
    {
        interactable.hoverEntered.AddListener(OnHoverEntered);
        interactable.hoverExited.AddListener(OnHoverExited);
        interactable.selectEntered.AddListener(OnSelectEntered);
    }

    private void OnDisable()
    {
        interactable.hoverEntered.RemoveListener(OnHoverEntered);
        interactable.hoverExited.RemoveListener(OnHoverExited);
        interactable.selectEntered.RemoveListener(OnSelectEntered);
    }

    private void Update()
    {
        if (isHovered && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("space selected settings option: " + actionName);
            TriggerAction();
        }
    }

    private void OnHoverEntered(HoverEnterEventArgs args)
    {
        isHovered = true;

        if (selector != null && optionText != null)
        {
            selector.position = new Vector3(
                selector.position.x,
                optionText.transform.position.y,
                selector.position.z
            );

            optionText.color = hoverColor;
        }
    }

    private void OnHoverExited(HoverExitEventArgs args)
    {
        isHovered = false;

        if (optionText != null)
            optionText.color = normalColor;
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        Debug.Log("selected settings option: " + actionName);
        TriggerAction();
    }

    private void TriggerAction()
    {
        switch (actionName)
        {
            case "Resume":
                settingsPanel.SetActive(false);
                break;

            case "Volume":
                mainSettingsView.SetActive(false);
                volumeView.SetActive(true);
                controlsView.SetActive(false);
                break;

            case "Controls":
                mainSettingsView.SetActive(false);
                volumeView.SetActive(false);
                controlsView.SetActive(true);
                break;

            case "Back":
                mainSettingsView.SetActive(true);
                volumeView.SetActive(false);
                controlsView.SetActive(false);
                break;

            case "MainMenu":
                SceneManager.LoadScene("MainMenu");
                break;

            default:
                Debug.LogWarning("No settings action assigned for: " + gameObject.name);
                break;
        }
    }
}