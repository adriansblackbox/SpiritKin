using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class equipmentShop : MonoBehaviour
{
    public GameObject triggerText;
    public GameObject Player;
    public GameObject equipMenu;
    public GameObject equipCamera;
    public PauseMenu pauseMenu;
    

    private bool isInteractable;

    public bool isOpen;
    public GameObject ShopFirstButton;

    // Start is called before the first frame update
    void Start()
    {
        isInteractable = false;
        isOpen = false;
        equipCamera.SetActive(false);
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
        //need condition to check if interactable
        // if()isInteractable = false;
        if ((Input.GetKeyDown(KeyCode.F) || Input.GetButtonDown("A Button"))&& isInteractable && !isOpen && !pauseMenu.GameIsPaused)
        {
            OpenMenu();
        }
        else if (Input.GetKeyDown(KeyCode.F) && isOpen && !pauseMenu.GameIsPaused)
        {
            CloseMenu();
        }
    }

    public void OpenMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        equipMenu.SetActive(true);
        //preload
        // equipMenu.GetComponent<NewShopManager>().Initial();
        equipCamera.SetActive(true);
        isOpen = true;

        //clear selected button
        EventSystem.current.SetSelectedGameObject(null);
        //reassign
        EventSystem.current.SetSelectedGameObject(ShopFirstButton);

        //disable player's script here
        Player.GetComponent<Animator>().SetFloat("Speed", 0.0f);
        Player.GetComponent<PlayerController>().enabled = false;
        //Player.GetComponent<PlayerCombat>().enabled = false;

        //disable UI
        //GameObject.FindWithTag("UI").GetComponent<CanvasGroup>().alpha = 0;
        Debug.Log (isOpen);
    }

    public void CloseMenu()
    {
        Player.GetComponent<PlayerController>().enabled = true;
        //Player.GetComponent<PlayerCombat>().enabled = true;
        //GameObject.FindWithTag("UI").GetComponent<CanvasGroup>().alpha = 1;
        Cursor.lockState = CursorLockMode.Locked;
        equipMenu.SetActive(false);
        equipCamera.SetActive(false);
        isOpen = false;
        Debug.Log (isOpen);
    }

    private void OnTriggerExit(Collider other)
    {
        isInteractable = false;
        triggerText.SetActive(false);
    }
}