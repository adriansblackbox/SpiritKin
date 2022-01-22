using UnityEngine;

public class Interactable : MonoBehaviour
{
    // public float radius = 3f;
    public GameObject triggerText;
    public Buff theBuff;
    // public PlayerStats thePlayer;
    private bool isInteractable;

    
    void Start(){
        theBuff.isApplied = false;
        triggerText.SetActive(false);
        isInteractable = false;
        
    }
    private void OnTriggerStay(Collider other){
        if(other.gameObject.tag =="Player"){
            isInteractable = true;
            triggerText.SetActive(true);
        }
    }
    void Update(){
        
        if(Input.GetKeyDown(KeyCode.F) && isInteractable){
                //do something;
                if(!theBuff.isApplied){
                    GameObject.Find("Player").GetComponent<PlayerStats>().Buffs.Add(theBuff);
                }
                
                // playerStats.Buffs.Add(theBuff);
                Debug.Log("ACTIVATED");
            }
    }
    private void OnTriggerExit(Collider other){
        isInteractable = false;
        triggerText.SetActive(false);
    }
    // void OnDrawGizmosSelected (){
    //     Gizmos.color = Color.yellow;
    //     Gizmos.DrawWireSphere(transform.position, radius);
    // }

}
