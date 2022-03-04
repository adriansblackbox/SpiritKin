using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TeaShop : MonoBehaviour
{
    public GameObject triggerText;

    public GameObject Player;

    public GameObject teaMenu;

    public GameObject teaCamera;

    public Buff theBuff;

    private bool isInteractable;

    private bool isOpen;
      public GameObject ShopFirstButton;

    // Start is called before the first frame update
    void Start()
    {
        isInteractable = false;
        isOpen = false;
        teaCamera.SetActive(false);

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
        if (Input.GetKeyDown(KeyCode.F) && isInteractable && !isOpen)
        {
            //do something;
            // if(!theBuff.isApplied){
            //     GameObject.Find("Player").GetComponent<PlayerStats>().Buffs.Add(theBuff);
            // }
            OpenMenu();
        }
        else if (Input.GetKeyDown(KeyCode.F) && isOpen)
        {
            CloseMenu();
        }
    }

    public void OpenMenu()
    {
        

        Cursor.lockState = CursorLockMode.None;
        teaMenu.SetActive(true);
        teaCamera.SetActive(true);
        isOpen = true;

        //clear selected button
        EventSystem.current.SetSelectedGameObject(null);
        //reassign
        EventSystem.current.SetSelectedGameObject(ShopFirstButton);

        //disable player's script here
        Player.GetComponent<Animator>().SetFloat("Speed", 0.0f);
        Player.GetComponent<PlayerController>().enabled = false;
        Player.GetComponent<PlayerCombat>().enabled = false;

        //disable UI
        GameObject.FindWithTag("UI").GetComponent<CanvasGroup>().alpha = 0;
        Debug.Log (isOpen);
    }

    public void CloseMenu()
    {
        Player.GetComponent<PlayerController>().enabled = true;
        Player.GetComponent<PlayerCombat>().enabled = true;
        GameObject.FindWithTag("UI").GetComponent<CanvasGroup>().alpha = 1;
        Cursor.lockState = CursorLockMode.Locked;
        teaMenu.SetActive(false);
        teaCamera.SetActive(false);
        isOpen = false;
        Debug.Log (isOpen);
    }

    private void OnTriggerExit(Collider other)
    {
        isInteractable = false;
        triggerText.SetActive(false);
    }
}
