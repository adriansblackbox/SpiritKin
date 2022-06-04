using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

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

    public GameObject LeftInstruct;
    public GameObject RightInstruct;

    public Sprite AButton;
    public Sprite DButton;
    public AudioClip shopenter;
    public AudioSource audioSource;

    public Sprite LeftBumperButton;
    public Sprite RightBumperButton;

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
            LeftInstruct.GetComponent<Image>().sprite = AButton;
            LeftInstruct.GetComponent<RectTransform>().sizeDelta = new Vector2(60, 60);
            RightInstruct.GetComponent<Image>().sprite = DButton;
            RightInstruct.GetComponent<RectTransform>().sizeDelta = new Vector2(60, 60);
            
            triggerText.GetComponent<TextMeshProUGUI>().text = "Press F to open shop";
            
            if(eventSystem.currentSelectedGameObject != null){
                // ShopFirstButton = eventSystem.currentSelectedGameObject;
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
        else
        {

            //enable texts
            LeftInstruct.GetComponent<Image>().sprite = LeftBumperButton;
            LeftInstruct.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 60);
            RightInstruct.GetComponent<Image>().sprite = RightBumperButton;
            RightInstruct.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 60);
            //get textmeshpro text
            triggerText.GetComponent<TextMeshProUGUI>().text = "Press Y to open shop";
            

            if (eventSystem.currentSelectedGameObject == null)
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject (ShopFirstButton);
            }
        }

        if (
            (Input.GetKeyDown(KeyCode.F) || Input.GetButtonDown("Y Button")) &&
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
        if (FindObjectOfType<ControlOverlayHandler>().keyboard)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        

        //disable player controller
        playerController.speed = 0;
        playerController.animator.SetFloat("Speed", 0);
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
        Enter();
        Debug.Log ("opened menu");
    }

    public void Enter()
    {
        audioSource.PlayOneShot(shopenter);
    }

    public void CloseMenu()
    {
        if (FindObjectOfType<ControlOverlayHandler>().keyboard)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        //GameObject.FindWithTag("UI").GetComponent<CanvasGroup>().alpha = 1;
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
