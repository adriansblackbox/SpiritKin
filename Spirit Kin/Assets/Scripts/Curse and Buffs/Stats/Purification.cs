using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class Purification : MonoBehaviour
{
    public enum PState {
        None,
        Touching,
        Cleared
    }

    public bool isCursed;
    private bool isPurifying;

    public Transform respawnPoint;
    public GameObject player;
    public Collider coll;
    public PState PurificationState;
    public float PurificationTime = 5.0f;
    public Image PurificationMeter;
    public VisualEffect[] VFX;
    public float HealingRate = 5f;

    // Start is called before the first frame update
    void Start()
    {
        foreach(VisualEffect vfx in VFX) {
            vfx.enabled = true;
        }
        PurificationState = PState.None;
        isCursed = false;
        isPurifying = false;
        PurificationMeter.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Temp VFX -> trying it with always running
        // if(FindObjectOfType<CurseMeter>().activeCurses.Count > 0)
        // {
        //     foreach(VisualEffect vfx in VFX) {
        //         vfx.enabled = true;
        //     }
        // }
        // else
        // {
        //     foreach(VisualEffect vfx in VFX) {
        //         vfx.enabled = false;
        //     }
        // }

        if(isPurifying){
            player.GetComponent<CurseMeter>().curseMeter -= Time.deltaTime/2f;
            player.GetComponent<CurseMeter>().curseMeter = Mathf.Clamp(player.GetComponent<CurseMeter>().curseMeter, 0, 1);
        }
        if(isPurifying && FindObjectOfType<CurseMeter>().activeCurses.Count > 0){
            PurificationMeter.fillAmount -= Time.deltaTime/PurificationTime;
        }
        if(PurificationMeter.fillAmount <= 0){
            player.GetComponent<CurseMeter>().removeCurse();
            PurificationMeter.fillAmount = 1f;
        }
        if(FindObjectOfType<CurseMeter>().activeCurses.Count == 0){
            PurificationMeter.enabled = false;
        }
        if(isPurifying && player.GetComponent<PlayerStats>().currentHealth < player.GetComponent<PlayerStats>().maxHealth) {
            player.GetComponent<PlayerStats>().currentHealth += Time.deltaTime * HealingRate;
        }
    }

    // Add : Purification shrines also restore HP?

    void OnTriggerEnter (Collider targetObj) {
        if (targetObj.gameObject.tag == "Player"){
            PurificationMeter.enabled = true;
            PurificationState = PState.Touching;
            isPurifying = true;
            PurificationMeter.fillAmount = 1f;
            Debug.Log("Starting to purify!");
        }
    }

    void OnTriggerExit (Collider targetObj) {
        if (targetObj.gameObject.tag == "Player"){
            PurificationState = PState.None;
            Debug.Log("Cancelled purification");
            PurificationMeter.fillAmount = 1f;
            PurificationMeter.enabled = false;
            isPurifying = false;
        }
    }
}
