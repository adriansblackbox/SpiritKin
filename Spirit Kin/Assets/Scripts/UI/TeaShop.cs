using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TeaShop : MonoBehaviour
{
    [SerializeField]
    PlayerController playerController;

    public GameObject triggerText;

    public GameObject Player;

    public GameObject teaMenu;

    public GameObject teaCamera;

    public PauseMenu pauseMenu;

    private bool isInteractable;

    public bool isOpen;

    public GameObject ShopFirstButton;

    public GameObject UI;

    public EventSystem eventSystem;

    public GameObject Instruct1;
    public GameObject Instruct2;

    // Start is called before the first frame update
    void Start()
    {
        isInteractable = false;

        teaCamera.SetActive(false);
        pauseMenu = GameObject.Find("PauseCanvas").GetComponent<PauseMenu>();

        //load buttons:
        //find position
        // GameObject.Find("Items").GetComponent
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isInteractable = true;
            if (!isOpen)
            {
                triggerText.SetActive(true);
            }
            else
            {
                triggerText.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (UI.GetComponent<ControlOverlayHandler>().keyboard)
        {   
            //disable texts
            Instruct1.SetActive(false);
            Instruct2.SetActive(false);
            
            if(eventSystem.currentSelectedGameObject != null){
                ShopFirstButton = eventSystem.currentSelectedGameObject;
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
        else
        {
            //enable texts
            Instruct1.SetActive(true);
            Instruct2.SetActive(true);
            if (eventSystem.currentSelectedGameObject == null)
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject (ShopFirstButton);
            }
        }

        if (
            (Input.GetKeyDown(KeyCode.F) || Input.GetButtonDown("X Button")) &&
            isInteractable &&
            !isOpen &&
            !pauseMenu.GameIsPaused
        )
        {
            OpenMenu();
        }
        else if (
            Input.GetKeyDown(KeyCode.F) && isOpen && !pauseMenu.GameIsPaused
        )
        {
            CloseMenu();
        }
    }

    public void reFresh()
    {
        // EventSystem.current.selectedGameObject = null;
    }
    public void OpenMenu()
    {
        // Cursor.lockState = CursorLockMode.None;

        //disable player controller
        playerController.enabled = false;
        teaMenu.SetActive(true);

        //preload
        // teaMenu.GetComponent<NewShopManager>().Initial();
        teaCamera.SetActive(true);

        //clear selected button
        EventSystem.current.SetSelectedGameObject(null);

        //reassign
        EventSystem.current.SetSelectedGameObject (ShopFirstButton);

        //disable UI
        //GameObject.FindWithTag("UI").GetComponent<CanvasGroup>().alpha = 0;
        //shop is now open
        isOpen = true;
        Debug.Log ("opened menu");
    }

    public void CloseMenu()
    {
        //enable player controller
        if(isOpen = false)
        

        //GameObject.FindWithTag("UI").GetComponent<CanvasGroup>().alpha = 1;
        // Cursor.lockState = CursorLockMode.Locked;
        teaMenu.SetActive(false);
        teaCamera.SetActive(false);
        isOpen = false;
        playerController.enabled = true;
        Debug.Log ("closed menu");
    }

    private void OnTriggerExit(Collider other)
    {
        isInteractable = false;
        triggerText.SetActive(false);
    }
}
