using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStats : MonoBehaviour
{
    public int coins;
    public int maxSouls;
    public int Coins;
    public int maxHealth = 100;
    public int currentHealth;

    public Stat armor;
    public Stat damage;
    public Stat speed;

    public Text SoulsUI;
    public ParticleSystem hitVFX;

    public GameObject deathScene;
    
    void Awake ()
    {
        currentHealth = maxHealth;
    }

    void Update() {
        
    }

    public void TakeDamage (int damage) {
        hitVFX.Play();
        //armor system
        damage -= armor.GetValue();
        damage = Mathf.Clamp(damage, 0, int.MaxValue);

        currentHealth -= damage;

        if (gameObject.tag == "Enemy" && gameObject.GetComponent<Enemy_Controller>().EnemyMotion != Enemy_Controller.MotionState.Chasing) {
            gameObject.GetComponent<Enemy_Controller>().EnemyMotion = Enemy_Controller.MotionState.Chasing;
        }

        if (currentHealth <= 0) {
            Die();
        }
    }

    public virtual void Die () {
        //Die in some way
        if (gameObject.tag == "Enemy") {
            GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterStats>().coins += coins;
            Destroy(this.gameObject);
        }
        if (FindObjectOfType<LockableTargets>()._possibleTargets.Contains(this.gameObject)) {
            FindObjectOfType<LockTarget>().DelockTarget();
        }
        if (FindObjectOfType<SwordCollision>().immuneEnemies.Contains(this.gameObject)) {
            FindObjectOfType<SwordCollision>().immuneEnemies.Remove(this.gameObject);
        }
        if (gameObject.tag =="Player"){
            deathScene.SetActive(true);
        }
        
    }
}
