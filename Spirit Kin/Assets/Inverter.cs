using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inverter : MonoBehaviour
{
    public float timer;
    public invertCurse invert;
    public bool inverted;
    public GameObject inverterUICanvas;
    public GameObject inverterUIImage;

    // Start is called before the first frame update
    void Start(){}

    void passInvertCurse (invertCurse i) {
        invert = i;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > invert.timerTime) {
            timer = 0;
            inverted = !inverted;
            if(inverted) {
                gameObject.SendMessage("InvertControls", false);
                inverterUIImage.transform.Rotate(0, 0, 0); // Replace with new UI thing later
            }
            else {
                gameObject.SendMessage("InvertControls", true);
                inverterUIImage.transform.Rotate(0, 0, 180); // Replace with new UI thing later
            }
        }
    }

    void LateUpdate () 
    {
        inverterUICanvas.transform.LookAt(Camera.main.transform);
        inverterUICanvas.transform.Rotate(0, 180, 0);
    }

    void OnEnable () {
        inverterUICanvas.SetActive(true);
        inverterUIImage.SetActive(true);
    }

    void OnDisable (){
        inverterUICanvas.SetActive(false);
        inverterUIImage.SetActive(true);
    }
}
