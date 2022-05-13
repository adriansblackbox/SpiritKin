using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordCollision : MonoBehaviour
{
    public float BladeLength;
    public LayerMask layerMask;
    public List<GameObject> immuneEnemies;
    public bool RaycastOn;
    public Transform AttackOriginPoints;
    public GameObject[] SwordVFX;
    private float baseLength;

    private void Start() {
        // x = 2.5/bladelength
        baseLength = BladeLength;
        ScaleVFXToBlade();
    }

    private void Update() {
        RaycastHit hit;
        //checking to see if we hit an enemy
        foreach(Transform child in AttackOriginPoints) {

            if (Physics.SphereCast(child.position, 1f,  child.TransformDirection(Vector3.forward), out hit, BladeLength, layerMask) && RaycastOn)
            {
                Debug.DrawRay(child.position, child.TransformDirection(Vector3.forward) * BladeLength, Color.red);
                if(!immuneEnemies.Contains(hit.transform.gameObject)){
                    immuneEnemies.Add(hit.transform.gameObject);
                    hit.transform.gameObject.GetComponent<CharacterStats>().TakeDamage(FindObjectOfType<PlayerStats>().damage.GetValue(), 5f);
                }
            }
            else if(RaycastOn)
            {
                Debug.DrawRay(child.position, child.TransformDirection(Vector3.forward) * BladeLength, Color.yellow);
            }
        }
    }
    public void ScaleVFXToBlade() {
        foreach(GameObject vfx in SwordVFX) {
            vfx.transform.localScale =   new Vector3(BladeLength, BladeLength, BladeLength) * (2.25f/baseLength);
        }
    }
}
