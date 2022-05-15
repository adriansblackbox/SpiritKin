using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordCollision : MonoBehaviour
{
    public float BladeLength;
    public LayerMask layerMask;
    public List<GameObject> immuneEnemies;
    public bool RaycastOn;
    public bool vampBlessingOn = false;
    public float vampAmount = 0f;
    public Transform AttackOriginPoints;
    public GameObject[] SwordVFX;

    private PlayerStats pStats;
    private float baseLength;

    private void Start() {
        pStats = FindObjectOfType<PlayerStats>();
        // x = 2.5/bladelength
        baseLength = BladeLength;
        ScaleVFXToBlade();
    }

    private void Update() {
        RaycastHit hit;
        
        //checking to see if we hit an enemy
        foreach(Transform child in AttackOriginPoints) {
            if (Physics.SphereCast(child.position, 1f,  child.TransformDirection(Vector3.forward), out hit, BladeLength, layerMask) && RaycastOn) {
                Debug.DrawRay(child.position, child.TransformDirection(Vector3.forward) * BladeLength, Color.red);
                if(!immuneEnemies.Contains(hit.transform.gameObject)) {
                    immuneEnemies.Add(hit.transform.gameObject);
                    hit.transform.gameObject.GetComponent<CharacterStats>().TakeDamage(pStats.damage.GetValue(), 5f);
                    if(vampBlessingOn) {
                        pStats.currentHealth = Mathf.Clamp(pStats.currentHealth + vampAmount * pStats.damage.GetValue(), -0.1f, pStats.maxHealth * pStats.currentHealthCap);
                    }
                }
            }
            if(GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.name == "StompFollow_K") {
                Debug.Log("360 Raycast!!");
                if (Physics.SphereCast(child.position, 1f,  child.TransformDirection(Vector3.forward * -1), out hit, BladeLength, layerMask) && RaycastOn) {
                    Debug.DrawRay(child.position, child.TransformDirection(Vector3.forward) * BladeLength, Color.red);
                    if(!immuneEnemies.Contains(hit.transform.gameObject)) {
                        immuneEnemies.Add(hit.transform.gameObject);
                        hit.transform.gameObject.GetComponent<CharacterStats>().TakeDamage(pStats.damage.GetValue(), 5f);
                        if(vampBlessingOn) {
                            pStats.currentHealth = Mathf.Clamp(pStats.currentHealth + vampAmount * pStats.damage.GetValue(), -0.1f, pStats.maxHealth * pStats.currentHealthCap);
                        }
                    }
                }
            }
            if(RaycastOn) {
                Debug.DrawRay(child.position, child.TransformDirection(Vector3.forward) * BladeLength, Color.yellow);
                if(GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.name == "StompFollow_K")
                    Debug.DrawRay(child.position, child.TransformDirection(Vector3.forward * -1) * BladeLength, Color.yellow);
            }
        }
    }

    public void ScaleVFXToBlade() {
        foreach(GameObject vfx in SwordVFX) {
            vfx.transform.localScale =   new Vector3(BladeLength, BladeLength, BladeLength) * (2.25f/baseLength);
        }
    }
}
