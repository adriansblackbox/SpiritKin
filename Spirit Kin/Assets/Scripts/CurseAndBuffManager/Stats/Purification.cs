using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Purification : MonoBehaviour
{
    public enum PState {
        None,
        Touching,
        Cleared
    }

    public bool isCursed;
    private bool isPurifying;

    public GameObject player;
    public Collider coll;
    public PState PurificationState;
    public float PurificationTime = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        PurificationState = PState.None;
        isCursed = false;
        isPurifying = false;
    }

    // Update is called once per frame
    void Update()
    {
        switch(PurificationState){ // Useful if theres stuff we wanna do every frame while colliding
            case PState.None :
                break;
            case PState.Touching :
                if(!isPurifying){
                    StartCoroutine(PurificationTimer()); // Continuous curing effect. Consider also docking the time a bit in this flow?
                }
                break;
            case PState.Cleared :
                break;
        } 
    }

    public IEnumerator PurificationTimer () {
        isPurifying = true;
        yield return new WaitForSeconds(PurificationTime);
        if(PurificationState == PState.Touching){
            if(isCursed) {
                // Leaving logic in here in case we decide to use it. Literally costs like 3 assembly commands.
            }
            else {
                //PurificationState = PState.Cleared; // Send signal to player to remove a curse from their array
                player.GetComponent<CurseMeter>().removeCurse;
            }
        }
        isPurifying = false;
    }

    // Add : Purification shrines also restore HP?

    void OnTriggerEnter (Collider targetObj) {
        if (targetObj.gameObject.tag == "Player"){
            PurificationState = PState.Touching;
            StartCoroutine(PurificationTimer());
            Debug.Log("Starting to purify!");
        }
    }

    void OnTriggerExit (Collider targetObj) {
        if (targetObj.gameObject.tag == "Player"){
            PurificationState = PState.None;
            StopCoroutine(PurificationTimer());
            Debug.Log("Cancelled purification");
        }
    }
}
