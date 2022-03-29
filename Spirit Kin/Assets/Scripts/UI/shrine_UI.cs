using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class shrine_UI : MonoBehaviour
{
    public GameObject Shrine;
    public Image Bar;
    private void Update() {
        if(Shrine.GetComponent<Shrine>().cursed)
            Bar.fillAmount = (Shrine.GetComponent<Shrine>().TotalCurseTime - Shrine.GetComponent<Shrine>().CurCurseTime) / Shrine.GetComponent<Shrine>().TotalCurseTime;
        else
            Bar.fillAmount = 1;
    }
}
