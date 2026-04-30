using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class DeliverButton : MonoBehaviour
{
    public DisplayManager displayManager;

    [Header("Visuals")]
    public Transform buttonVisual;
    public Renderer buttonRenderer;

    [Header("Colors")]
    public Color normalColor = new Color(0.55f, 0.23f, 0.18f);
    public Color hoverColor = new Color(0.72f, 0.32f, 0.25f);
    public Color pressedColor = new Color(0.35f, 0.15f, 0.12f);

    [Header("Scale")]
    public Vector3 normalScale = new Vector3(0.55f, 0.28f, 0.03f);
    public Vector3 hoverScale = new Vector3(0.58f, 0.30f, 0.033f);
    public Vector3 pressedScale = new Vector3(0.52f, 0.26f, 0.027f);

    [Header("Animation")]
    public float animationSpeed = 14f;

    private XRSimpleInteractable interactable;
    private Material runtimeMaterial;
    private Vector3 targetScale;
    private Color targetColor;
    private bool isHovered = false;

    private void Awake()
    {
        interactable = GetComponent<XRSimpleInteractable>();

        if (buttonVisual == null)
            buttonVisual = transform;

        if (buttonRenderer == null)
            buttonRenderer = GetComponent<Renderer>();

        if (buttonRenderer != null)
            runtimeMaterial = buttonRenderer.material;

        targetScale = normalScale;
        targetColor = normalColor;
    }

    private void OnEnable()
    {
        if (interactable == null) return;

        interactable.hoverEntered.AddListener(OnHoverEntered);
        interactable.hoverExited.AddListener(OnHoverExited);
        interactable.selectEntered.AddListener(OnSelectEntered);
        interactable.selectExited.AddListener(OnSelectExited);
    }

    private void OnDisable()
    {
        if (interactable == null) return;

        interactable.hoverEntered.RemoveListener(OnHoverEntered);
        interactable.hoverExited.RemoveListener(OnHoverExited);
        interactable.selectEntered.RemoveListener(OnSelectEntered);
        interactable.selectExited.RemoveListener(OnSelectExited);
    }

    private void Start()
    {
        if (buttonVisual != null)
            buttonVisual.localScale = normalScale;

        if (runtimeMaterial != null)
            runtimeMaterial.color = normalColor;
    }

    private void Update()
    {
        if (buttonVisual != null)
        {
            buttonVisual.localScale = Vector3.Lerp(
                buttonVisual.localScale,
                targetScale,
                Time.deltaTime * animationSpeed
            );
        }

        if (runtimeMaterial != null)
        {
            runtimeMaterial.color = Color.Lerp(
                runtimeMaterial.color,
                targetColor,
                Time.deltaTime * animationSpeed
            );
        }

        // desktop fallback test:
        // if the button is hovered and you press space, trigger the action
        if (isHovered && Input.GetKeyDown(KeyCode.Space))
        {
            PressAndHandleAction();
        }
    }

    private void OnHoverEntered(HoverEnterEventArgs args)
    {
        isHovered = true;
        targetScale = hoverScale;
        targetColor = hoverColor;
    }

    private void OnHoverExited(HoverExitEventArgs args)
    {
        isHovered = false;
        targetScale = normalScale;
        targetColor = normalColor;
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        Debug.Log("OnSelectEntered triggered");
        PressAndHandleAction();
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        targetScale = hoverScale;
        targetColor = hoverColor;
    }

    private void PressAndHandleAction()
    {
        Debug.Log("PressAndHandleAction called");
        targetScale = pressedScale;
        targetColor = pressedColor;
        HandleAction();
    }

    private void HandleAction()
    {
        Debug.Log("HandleAction called - checking displayManager");
        
        if (displayManager == null)
        {
            Debug.LogError("DisplayManager is not assigned on DeliverButton.");
            return;
        }

        Debug.Log("DisplayManager found. Calling SubmitCurrentOrder()");
        displayManager.SubmitCurrentOrder();
    }
}