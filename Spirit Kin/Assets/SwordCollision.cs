using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordCollision : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Enemy")){
            Debug.Log("HIT!");
            //other.GetComponent<CharacterStats>().TakeDamage(damage.GetValue());
        }
    }
}
