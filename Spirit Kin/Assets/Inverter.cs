using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inverter : MonoBehaviour
{
    public float timer;
    public invertCurse invert;
    public bool inverted;

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
            if(inverted) gameObject.SendMessage("InvertControls", false);
            else gameObject.SendMessage("InvertControls", true);
        }
    }
}
