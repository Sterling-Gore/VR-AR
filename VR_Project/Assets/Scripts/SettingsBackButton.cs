using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRSimpleInteractable))]
public class SettingsBackButton : MonoBehaviour
{
    public GameObject mainSettingsView;
    public GameObject volumeView;
    public GameObject controlsView;

    public TMP_Text buttonText;

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
        // desktop testing fallback
        if (isHovered && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("space selected back button");
            BackToMainSettings();
        }
    }

    private void OnHoverEntered(HoverEnterEventArgs args)
    {
        isHovered = true;

        if (buttonText != null)
            buttonText.color = hoverColor;
    }

    private void OnHoverExited(HoverExitEventArgs args)
    {
        isHovered = false;

        if (buttonText != null)
            buttonText.color = normalColor;
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        Debug.Log("XR selected back button");
        BackToMainSettings();
    }

    private void BackToMainSettings()
    {
        mainSettingsView.SetActive(true);
        volumeView.SetActive(false);
        controlsView.SetActive(false);
    }
}