using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordCollision : MonoBehaviour
{
    public float BladeLength = 10f;
    public LayerMask layerMask;
    public List<GameObject> immuneEnemies;
    public bool RaycastOn;
    public Transform[] AttackOriginPoints;
    PlayerCombat combatScript;
    private void Start() {
       combatScript = FindObjectOfType<PlayerCombat>();
       AttackOriginPoints = FindObjectOfType<PlayerCombat>().AttackOriginPoints;
       immuneEnemies = FindObjectOfType<PlayerCombat>().immuneEnemies;
    }
    private void Update() {
        RaycastHit hit;
        //checking to see if we hit an enemy
        foreach(Transform originPoint in AttackOriginPoints){

            if (Physics.SphereCast(originPoint.position, 1f,  originPoint.TransformDirection(Vector3.forward), out hit, BladeLength, layerMask) && RaycastOn)
            {
                Debug.DrawRay(originPoint.position, originPoint.TransformDirection(Vector3.forward) * BladeLength, Color.red);
                if(!immuneEnemies.Contains(hit.transform.gameObject)){
                    immuneEnemies.Add(hit.transform.gameObject);
                    hit.transform.gameObject.GetComponent<CharacterStats>().TakeDamage(FindObjectOfType<PlayerStats>().damage.GetValue());
                }
            }
            else if(RaycastOn)
            {
                Debug.DrawRay(originPoint.position, originPoint.TransformDirection(Vector3.forward) * BladeLength, Color.yellow);
            }
        }
    }
    public void EnableHitRay(){
        RaycastOn = true;
    }
    public void DisableHitRay(){
        RaycastOn = false;
    }
}
