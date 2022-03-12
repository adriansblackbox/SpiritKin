using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage_Player : MonoBehaviour
{
    public Enemy_Controller enemy_Controller;
    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player" && enemy_Controller.currentAttack.name == "Charge" && enemy_Controller.attackTimer != 0) //need a more robust way to test
        {
            Debug.Log("Player was hit");
            //player takes 10 damage
            col.GetComponent<PlayerStats>().TakeDamage(20);
            //update health bar
        }
    }
}
