using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoroutineHandler : MonoBehaviour {
    public float timerTime;
    public bool active;
    public GameObject player;
    public PlayerController pControl;

    private IEnumerator curseFlipFlop () {
        Debug.Log("Woooo!");
        while (active) {
            Debug.Log("Woooo2!");
            if (pControl.getInverted()) player.SendMessage("InvertControls", false); // Ternary statement seems to be very angry with this.
            else player.SendMessage("InvertControls", true);
            yield return new WaitForSeconds (timerTime);
            Debug.Log("Woooo3!");
        }

        player.SendMessage("InvertControls", false);
    }

    public void StartCurseCoroutine(){
        //Debug.Log("Woooo0!");
        //StartCoroutine(curseFlipFlop());
    }

    public void StopCurseCoroutine(){
        //StopCoroutine(curseFlipFlop());
    }

    void Update(){}
}

public class invertCurse : Curse
{
    private GameObject player;
    private PlayerController pControl;
    private float timerTime;
    private CoroutineHandler recurser;

    public invertCurse (Sprite _image, GameObject _player) {
        recurser = new CoroutineHandler();
        type = "invert_Curse";
        isApplied = false;
        removeFlag = false;
        image = _image;
        player = recurser.player = _player;
        pControl = recurser.pControl = player.GetComponent<PlayerController>();

        timerTime = 1000.0f;
    }

    override public void invokeCurse () {
        Debug.Log(type + " Added!");
        isApplied = true;
        player.GetComponent<CurseMeter>().activeCurses.Add(this);

        player.SendMessage("InvertControls", true);
        recurser.active = active;
        recurser.timerTime = timerTime;
        
        recurser.StartCurseCoroutine();
    } 

    override public void removeCurse () {
        removeFlag = false;
        isApplied = false;
        active = false;

        recurser.active = active;
        recurser.StopCurseCoroutine();

        player.SendMessage("InvertControls", false);
    } 
}
