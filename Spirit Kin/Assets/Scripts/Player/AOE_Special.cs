using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOE_Special : MonoBehaviour
{
    public GameObject Collider;
    private void Start() {
        Collider.SetActive(false);
    }
    private void Update() {
        if(Input.GetKeyDown(KeyCode.Mouse1)){
            Collider.SetActive(true);
        }
        if(Input.GetKeyUp(KeyCode.Mouse1)){
            Collider.SetActive(false);
        }
    }
    private void OnTriggerEnter(Collider other) {
        //if(other.gameObject.CompareTag("Enemy")){
        //    other.gameObject.GetComponent<CharacterStats>().TakeDamage(50f);
        //}
    }
}
