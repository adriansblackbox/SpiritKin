using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MainHub : MonoBehaviour
{
    public bool playerInHub = false;
    public CinemachineVirtualCamera mainHubCam;
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Player")){
            playerInHub = true;
            mainHubCam.Priority = 100;
        }
    }
    private void OnTriggerExit(Collider other) {
        if(other.gameObject.CompareTag("Player")){
            playerInHub = false;
            mainHubCam.Priority = 0;
        }
    }
}
