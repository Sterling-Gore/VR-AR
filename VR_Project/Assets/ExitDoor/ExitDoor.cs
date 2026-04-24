using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.SceneManagement;

public class ExitDoor : MonoBehaviour
{
    [SerializeField] InputActionReference rightControllerTrigger;
    [SerializeField] InputActionReference leftControllerTrigger;
    [SerializeField] private XRGrabInteractable xrGrab;
    [SerializeField] private GameObject ui;
    private bool LeftControllerUsed = false;
    private bool rightControllerUsed = false;
    

//---------------------------------------------------------------//
/*                      Unity Functions                          */
    void Awake()
    {
        ui.SetActive(false);
        xrGrab.trackPosition = false;
        xrGrab.trackRotation = false;
    }
    
    void Start()
    {
        rightControllerTrigger.action.started += RightSnapOn;
        leftControllerTrigger.action.started += LeftSnapOn;
        xrGrab.selectEntered.AddListener(OnGrabbed);
        xrGrab.selectExited.AddListener(OnReleased);
        xrGrab.hoverEntered.AddListener(OnHoverEnter);
        xrGrab.hoverExited.AddListener(OnHoverExit);
    }

//---------------------------------------------------------------//
/*                      Private Functions                        */
    private void ExitGame()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void RightSnapOn(InputAction.CallbackContext context)
    {
        if (rightControllerUsed)
            ExitGame();
    }

    private void LeftSnapOn(InputAction.CallbackContext context)
    {
        if (LeftControllerUsed)
            ExitGame();
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        IXRSelectInteractor interactor = args.interactorObject;
        
        if (interactor.transform.CompareTag("LeftHand"))
        {
            LeftControllerUsed = true;
        }
        else if (interactor.transform.CompareTag("RightHand"))
        {
            rightControllerUsed = true;
        }
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        IXRSelectInteractor interactor = args.interactorObject;

        if (interactor.transform.CompareTag("LeftHand"))
        {
            LeftControllerUsed = false;
        }
        else if (interactor.transform.CompareTag("RightHand"))
        {
            rightControllerUsed = false;
        }
    }

    private void OnHoverEnter(HoverEnterEventArgs args)
    {
        ui.SetActive(true);
    }

    private void OnHoverExit(HoverExitEventArgs args)
    {
        ui.SetActive(false);
    }
}
