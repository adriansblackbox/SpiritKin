using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStats : MonoBehaviour
{
    public int currSouls;
    public int maxSouls;
    public int Coins;
    public int maxHealth = 100;
    public int currentHealth;

    public Stat armor;
    public Stat damage;
    public Stat speed;

    public Text SoulsUI;
    public Text CoinsUI;
    public Material blue;
    public Material red;
    
    void Awake ()
    {
        currentHealth = maxHealth;
        //if(gameObject.tag != "Player")
        //{
        //    enabled = false; // Disables the update function for non-players. Collision still triggers.
        //}
    }

    void Update() {

            
        
        
        // if(!gameObject.CompareTag("Player")){
        //     Debug.Log("isEnemy");
        //     if(FindObjectOfType<SwordCollision>().immuneEnemies.Contains(this.gameObject)){
        //         Debug.Log("isBlue");
        //         GetComponent<MeshRenderer>().material = blue;
        //     }else{
        //         Debug.Log("isRed");
        //         GetComponent<MeshRenderer>().material = red;
        //     }
        // }
    }

    public void TakeDamage (int damage){
        //armor system
        damage -= armor.GetValue();
        damage = Mathf.Clamp(damage, 0, int.MaxValue);

        currentHealth -= damage;
        //Debug.Log(transform.name + " takes " + damage + " damage.");

        if (currentHealth <= 0){
            Die();
        }
    }

    public virtual void Die (){
        //Die in some way
        if(gameObject.tag == "Enemy") {
            GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterStats>().currSouls += currSouls;
        }
        if(FindObjectOfType<LockableTargets>()._possibleTargets.Contains(this.gameObject)){
            FindObjectOfType<LockableTargets>()._possibleTargets.Remove(this.gameObject);
        }
        if( FindObjectOfType<SwordCollision>().immuneEnemies.Contains(this.gameObject)){
             FindObjectOfType<SwordCollision>().immuneEnemies.Remove(this.gameObject);
        }
        Destroy(this.gameObject);
    }
}
