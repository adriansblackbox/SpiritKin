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
    PlayerCombat combatScript;
    private void Start() {
       SwordTrail.SetActive(false);
       combatScript = FindObjectOfType<PlayerCombat>();
    }
    private void Update() {
        RaycastHit hit;
        //checking to see if we hit an enemy
        if (Physics.Raycast(BladeRayOrigin.position, BladeRayOrigin.TransformDirection(Vector3.left), out hit, BladeLength, layerMask) && SwordTrail.activeSelf)
        {
            Debug.DrawRay(BladeRayOrigin.position, BladeRayOrigin.TransformDirection(Vector3.left) * BladeLength, Color.red);
            if(!immuneEnemies.Contains(hit.transform.gameObject)){
                immuneEnemies.Add(hit.transform.gameObject);
                hit.transform.gameObject.GetComponent<CharacterStats>().TakeDamage(1);
            }
        }
        else if(SwordTrail.activeSelf)
        {
            Debug.DrawRay(BladeRayOrigin.position, BladeRayOrigin.TransformDirection(Vector3.left) * BladeLength, Color.yellow);
        }
    }
    public void activateSword(){
        SwordTrail.SetActive(true);
    }
    public void deactivateSword(){
        SwordTrail.SetActive(false);
    }
}
