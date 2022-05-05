using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class blindCurse : Curse
{
    private GameObject blindVignette;
    private GameObject[] shrineUI;
    private GameObject healthBar;
    private CurseMeter cMeter;
    
    public blindCurse (Sprite _image, CurseMeter _cMeter, GameObject[] _shrines, GameObject _blindVignette, GameObject _healthBar)
    {
        type = "Blind_Curse";
        isApplied = false;
        removeFlag = false;
        image = _image;
        cMeter = _cMeter;

        blindVignette = _blindVignette;
        shrineUI = _shrines;
        healthBar = _healthBar;
    }

    override public void invokeCurse () 
    {
        Debug.Log(type + " Added!");
        isApplied = true;
        cMeter.activeCurses.Add(this);

        foreach (GameObject i in shrineUI) {
            i.SetActive(false);
        }
        healthBar.SetActive(false);
        blindVignette.SetActive(true);
    } 

    override public void removeCurse () 
    {
        foreach (GameObject i in shrineUI) {
            i.SetActive(true);
        }
        healthBar.SetActive(true);
        blindVignette.SetActive(false);
    } 
}
