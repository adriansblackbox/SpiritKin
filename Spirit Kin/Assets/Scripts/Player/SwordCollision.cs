using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordCollision : MonoBehaviour
{
    public Transform BladeRayOrigin;
    public float BladeLength = 10f;
    public LayerMask layerMask;
    public List<GameObject> immuneEnemies = new List<GameObject>();
    private void Update() {
        RaycastHit hit;
        //checking to see if we hit an enemy
        if (Physics.Raycast(BladeRayOrigin.position, BladeRayOrigin.TransformDirection(Vector3.left), out hit, BladeLength, layerMask) && FindObjectOfType<PlayerCombat>().isAttacking)
        {
            Debug.DrawRay(BladeRayOrigin.position, BladeRayOrigin.TransformDirection(Vector3.left) * BladeLength, Color.red);
            if(!immuneEnemies.Contains(hit.transform.gameObject)){
                immuneEnemies.Add(hit.transform.gameObject);
                hit.transform.gameObject.GetComponent<CharacterStats>().TakeDamage(1);
            }
        }
        else
        {
            Debug.DrawRay(BladeRayOrigin.position, BladeRayOrigin.TransformDirection(Vector3.left) * BladeLength, Color.yellow);
        }
    }
}
