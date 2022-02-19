using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class TeaShop : MonoBehaviour
{
    public GameObject triggerText;
    public GameObject Player;
    public GameObject teaMenu;
    public Buff theBuff;
    private bool isInteractable;
    private bool isOpen;
    // Start is called before the first frame update
    void Start()
    {
        isInteractable = false;
        isOpen = false;

        //load buttons:
        //find position
        // GameObject.Find("Items").GetComponent
        
    }
    private void OnTriggerStay(Collider other){
        if(other.gameObject.tag =="Player"){
            isInteractable = true;
            if(!isOpen){
                triggerText.SetActive(true);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        //need condition to check if interactable
        // if()isInteractable = false;
        if(Input.GetKeyDown(KeyCode.F) && isInteractable &&!isOpen){
                //do something;
                // if(!theBuff.isApplied){
                //     GameObject.Find("Player").GetComponent<PlayerStats>().Buffs.Add(theBuff);
                // }
                
                Cursor.lockState = CursorLockMode.None;
                teaMenu.SetActive(true);
                isOpen = true;
                //disable player's script here
                Player.GetComponent<PlayerController>().enabled = false;
                Player.GetComponent<PlayerCombat>().enabled = false;
                //disable UI
                GameObject.FindWithTag("UI").GetComponent<CanvasGroup>().alpha = 0;
                Debug.Log(isOpen);
            }
        else if(Input.GetKeyDown(KeyCode.F) && isOpen){
            Player.GetComponent<PlayerController>().enabled = true;
            Player.GetComponent<PlayerCombat>().enabled = true;
            GameObject.FindWithTag("UI").GetComponent<CanvasGroup>().alpha = 1;
            Cursor.lockState = CursorLockMode.Locked;
            teaMenu.SetActive(false);
            isOpen = false;
            Debug.Log(isOpen);
        }
        

    }

    private void OnTriggerExit(Collider other){
        isInteractable = false;
        triggerText.SetActive(false);
    }
}
