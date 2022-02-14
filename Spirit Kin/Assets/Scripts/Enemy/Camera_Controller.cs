using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Camera_Controller : MonoBehaviour
{
    public GameObject ShrineCam;
    public GameObject PlayerCam;

    public Button UpButton;
    public Button RightButton;
    public Button DownButton;
    public Button LeftButton;
    public Button PlayerButton;

    // Start is called before the first frame update
    void Start()
    {
        UpButton.onClick.AddListener(() => swapCameras("Up"));
        RightButton.onClick.AddListener(() => swapCameras("Right"));
        DownButton.onClick.AddListener(() => swapCameras("Down"));
        LeftButton.onClick.AddListener(() => swapCameras("Left"));
        PlayerButton.onClick.AddListener(() => swapCameras("Player"));
    }

    public void Update(){
        if(Input.GetKey(KeyCode.UpArrow)){
            ShrineCam.SetActive(true);
            PlayerCam.SetActive(false);
            ShrineCam.transform.position = new Vector3(0, ShrineCam.transform.position.y, 300);  
        }

        if(Input.GetKey(KeyCode.DownArrow)){
            ShrineCam.SetActive(true);
            PlayerCam.SetActive(false);
            ShrineCam.transform.position = new Vector3(0, ShrineCam.transform.position.y, -300);    
        }

        if(Input.GetKey(KeyCode.LeftArrow)){
            ShrineCam.SetActive(true);
            PlayerCam.SetActive(false);
            ShrineCam.transform.position = new Vector3(-300, ShrineCam.transform.position.y, 0);   
        }

        if(Input.GetKey(KeyCode.RightArrow)){
            ShrineCam.SetActive(true); 
            PlayerCam.SetActive(false);
            ShrineCam.transform.position = new Vector3(300, ShrineCam.transform.position.y, 0);  
        }

        if(Input.GetKey(KeyCode.P)){
            ShrineCam.SetActive(false);
            PlayerCam.SetActive(true);
        }
    }

    public void swapCameras(string dir)
    {
        if (dir == "Player")
        {
            ShrineCam.SetActive(false);
            PlayerCam.SetActive(true);
        }
        else if (dir == "Up")
        {
            ShrineCam.SetActive(true);
            PlayerCam.SetActive(false);
            ShrineCam.transform.position = new Vector3(0, ShrineCam.transform.position.y, 300);            
        }
        else if (dir == "Right")
        {
            ShrineCam.SetActive(true);            
            PlayerCam.SetActive(false);
            ShrineCam.transform.position = new Vector3(300, ShrineCam.transform.position.y, 0);       
        }
        else if (dir == "Down")
        {
            ShrineCam.SetActive(true);
            PlayerCam.SetActive(false);
            ShrineCam.transform.position = new Vector3(0, ShrineCam.transform.position.y, -300);            
        }
        else if (dir == "Left")
        {
            ShrineCam.SetActive(true);
            PlayerCam.SetActive(false);
            ShrineCam.transform.position = new Vector3(-300, ShrineCam.transform.position.y, 0);            
        }
    }
}
