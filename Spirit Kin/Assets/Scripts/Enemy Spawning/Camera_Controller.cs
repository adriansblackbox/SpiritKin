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

    public void swapCameras(string dir)
    {
        if (dir == "Player")
        {
            ShrineCam.GetComponent<Camera>().enabled = false;
            PlayerCam.GetComponent<Camera>().enabled = true;
        }
        else if (dir == "Up")
        {
            ShrineCam.GetComponent<Camera>().enabled = true;
            PlayerCam.GetComponent<Camera>().enabled = false;
            ShrineCam.transform.position = new Vector3(0, ShrineCam.transform.position.y, 300);            
        }
        else if (dir == "Right")
        {
            ShrineCam.GetComponent<Camera>().enabled = true;
            PlayerCam.GetComponent<Camera>().enabled = false;
            ShrineCam.transform.position = new Vector3(300, ShrineCam.transform.position.y, 0);       
        }
        else if (dir == "Down")
        {
            ShrineCam.GetComponent<Camera>().enabled = true;
            PlayerCam.GetComponent<Camera>().enabled = false;
            ShrineCam.transform.position = new Vector3(0, ShrineCam.transform.position.y, -300);            
        }
        else if (dir == "Left")
        {
            ShrineCam.GetComponent<Camera>().enabled = true;
            PlayerCam.GetComponent<Camera>().enabled = false;
            ShrineCam.transform.position = new Vector3(-300, ShrineCam.transform.position.y, 0);            
        }
    }
}
