using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRSimpleInteractable))]
public class MenuButton3D : MonoBehaviour
{
    [Header("Button Type")]
    public string actionName;

    [Header("Audio")]
    [SerializeField] private MainMenuAudioManager audioManager;
    [SerializeField] private float soundDelay = 0.75f;

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
    private bool actionStarted = false;

    private void Awake()
    {
        interactable = GetComponent<XRSimpleInteractable>();

        if (audioManager == null)
            audioManager = FindFirstObjectByType<MainMenuAudioManager>();

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

        Debug.Log("menubutton3d started on " + gameObject.name);
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

        if (isHovered && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("space pressed while hovering on " + gameObject.name);
            PressAndHandleAction();
        }
    }

    private void OnHoverEntered(HoverEnterEventArgs args)
    {
        Debug.Log("hover entered on " + gameObject.name);
        isHovered = true;
        targetScale = hoverScale;
        targetColor = hoverColor;
    }

    private void OnHoverExited(HoverExitEventArgs args)
    {
        Debug.Log("hover exited on " + gameObject.name);
        isHovered = false;
        targetScale = normalScale;
        targetColor = normalColor;
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        Debug.Log("select entered on " + gameObject.name);
        PressAndHandleAction();
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        Debug.Log("select exited on " + gameObject.name);
        targetScale = hoverScale;
        targetColor = hoverColor;
    }

    private void PressAndHandleAction()
    {
        if (actionStarted)
            return;

        actionStarted = true;

        targetScale = pressedScale;
        targetColor = pressedColor;
        HandleAction();
    }

    private void HandleAction()
    {
        Debug.Log("handleaction called with actionname = " + actionName);

        switch (actionName)
        {
            case "StartShift":
                StartCoroutine(StartShiftAfterSound());
                break;

            case "Settings":
                Debug.Log("open settings/help panels here later");
                actionStarted = false;
                break;

            case "ClockOut":
                StartCoroutine(ClockOutAfterSound());
                break;

            default:
                Debug.LogWarning("no action assigned for button: " + gameObject.name);
                actionStarted = false;
                break;
        }
    }

    private IEnumerator StartShiftAfterSound()
    {
        audioManager?.PlayStartShiftClick();

        yield return new WaitForSeconds(soundDelay);

        Debug.Log("loading via loading screen");
        LoadingData.sceneToLoad = "big scene for big boys and girls";
        SceneManager.LoadScene("LoadingScene");
    }

    private IEnumerator ClockOutAfterSound()
    {
        audioManager?.PlayClockOutClick();

        yield return new WaitForSeconds(soundDelay);

        Debug.Log("quitting game");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}