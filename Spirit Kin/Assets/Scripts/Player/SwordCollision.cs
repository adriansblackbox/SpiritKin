using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordCollision : MonoBehaviour
{
    public Transform BladeRayOrigin;
    public float BladeLength = 10f;
    public LayerMask layerMask;
    public List<GameObject> immuneEnemies = new List<GameObject>();
    public GameObject SwordTrail;
    public Transform[] AttackOriginPoints;
    PlayerCombat combatScript;
    private void Start() {
       SwordTrail.SetActive(false);
       combatScript = FindObjectOfType<PlayerCombat>();
       AttackOriginPoints = FindObjectOfType<PlayerCombat>().AttackOriginPoints;
    }
    private void Update() {
        RaycastHit hit;
        //checking to see if we hit an enemy
        foreach(Transform originPoint in AttackOriginPoints){

            if (Physics.SphereCast(originPoint.position, 1f,  originPoint.TransformDirection(Vector3.forward), out hit, BladeLength, layerMask) && SwordTrail.activeSelf)
            {
                Debug.DrawRay(originPoint.position, originPoint.TransformDirection(Vector3.forward) * BladeLength, Color.red);
                if(!immuneEnemies.Contains(hit.transform.gameObject)){
                    immuneEnemies.Add(hit.transform.gameObject);
                    hit.transform.gameObject.GetComponent<CharacterStats>().TakeDamage(FindObjectOfType<PlayerStats>().damage.GetValue());
                }
            }
            else if(SwordTrail.activeSelf)
            {
                Debug.DrawRay(originPoint.position, originPoint.TransformDirection(Vector3.forward) * BladeLength, Color.yellow);
            }
        }
    }
    public void activateSword(){
        SwordTrail.SetActive(true);
    }
    public void deactivateSword(){
        SwordTrail.SetActive(false);
    }
}
