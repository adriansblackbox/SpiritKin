using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{

    public int maxHealth = 100;
    public int currentHealth;

    public Stat armor;
    
    public Stat damage;

    public Stat speed;
    
    void Awake ()
    {
        currentHealth = maxHealth;
    }

    void Update (){
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
        
        Debug.Log(transform.name + " died.");
        Destroy(this.gameObject);
    }
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Player Sword")){
            TakeDamage(34);
        }
    }
}
