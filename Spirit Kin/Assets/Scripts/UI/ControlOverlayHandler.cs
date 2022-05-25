using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlOverlayHandler : MonoBehaviour
{
    public GameObject MouseKeyboardOverlay;
    public GameObject XboxControllerOverlay;
    
    public Sprite controllerIndicator;
    public Sprite keyboardIndicator;

    public GameObject nextLineIndicator;

    public bool keyboard = false;
    void Update()
    {
        if (
            Input.GetKey(KeyCode.JoystickButton0) || Input.GetKey(KeyCode.JoystickButton1) || Input.GetKey(KeyCode.JoystickButton2) || 
            Input.GetKey(KeyCode.JoystickButton3) || Input.GetKey(KeyCode.JoystickButton4) || Input.GetKey(KeyCode.JoystickButton5) || 
            Input.GetKey(KeyCode.JoystickButton6) || Input.GetKey(KeyCode.JoystickButton7) || Input.GetKey(KeyCode.JoystickButton8) || 
            Input.GetKey(KeyCode.JoystickButton9) || Input.GetKey(KeyCode.JoystickButton10) || Input.GetKey(KeyCode.JoystickButton11) || 
            Input.GetKey(KeyCode.JoystickButton12) || Input.GetKey(KeyCode.JoystickButton13) || Input.GetKey(KeyCode.JoystickButton14) || 
            Input.GetKey(KeyCode.JoystickButton15) || Input.GetKey(KeyCode.JoystickButton16) || Input.GetKey(KeyCode.JoystickButton17) || 
            Input.GetKey(KeyCode.JoystickButton18) || Input.GetKey(KeyCode.JoystickButton19) || 
            
            Mathf.Abs(Input.GetAxis("RightStick X")) > 0.1f || Mathf.Abs(Input.GetAxis("RightStick Y")) > 0.1f || Mathf.Abs(Input.GetAxis("LeftStick X")) > 0.1f || 
            Mathf.Abs(Input.GetAxis("LeftStick Y")) > 0.1f || Mathf.Abs(Input.GetAxis("Right Trigger")) > 0.1f || Mathf.Abs(Input.GetAxis("Left Trigger")) > 0.1f ||
            Mathf.Abs(Input.GetAxis("Dpad X")) > 0.1f || Mathf.Abs(Input.GetAxis("Dpad Y")) > 0.1f
            )
        {
            keyboard = false;
        }
        else if (Input.anyKey || Mathf.Abs(Input.GetAxis("Mouse X")) > 0.1f || Mathf.Abs(Input.GetAxis("Mouse Y")) > 0.1f)
        {
            keyboard = true;
        }

        if (keyboard)
        {
            Cursor.lockState = CursorLockMode.None;
            MouseKeyboardOverlay.SetActive(true);
            XboxControllerOverlay.SetActive(false);
            nextLineIndicator.GetComponent<Image>().sprite = keyboardIndicator;
            nextLineIndicator.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 65);
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            MouseKeyboardOverlay.SetActive(false);
            XboxControllerOverlay.SetActive(true);
            nextLineIndicator.GetComponent<Image>().sprite = controllerIndicator;
            nextLineIndicator.GetComponent<RectTransform>().sizeDelta= new Vector2(50, 50);
        }
    }
}
