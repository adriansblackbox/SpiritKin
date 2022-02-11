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
    
    void Awake ()
    {
        currentHealth = maxHealth;
        if(gameObject.tag != "Player")
        {
            enabled = false; // Disables the update function for non-players. Collision still triggers.
        }
    }

    void Update() {
        SoulsUI.text = currSouls + "/" + maxHealth;
        CoinsUI.text = "" + Coins;
        if (currSouls > maxSouls)
        {
            currSouls = maxSouls;
        }
    }

    public void TakeDamage (int damage){
        //armor system
        damage -= armor.GetValue();
        damage = Mathf.Clamp(damage, 0, int.MaxValue);

        currentHealth -= damage;
        Debug.Log(transform.name + " takes " + damage + " damage.");

        if (currentHealth <= 0){
            Die();
        }
    }

    public virtual void Die (){
        //Die in some way
        if(gameObject.tag == "Enemy") {
            GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterStats>().currSouls += currSouls;
        }

        Debug.Log(transform.name + " died.");
        Destroy(this.gameObject);
    }
    
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Player Sword")){
            TakeDamage(damage.GetValue());
        }
    }
}
