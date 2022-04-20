using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStats : MonoBehaviour
{
    public int coins;
    public float maxHealth = 100;
    public float currentHealth;
    public bool isDying = false;

    public Stat armor;
    public Stat damage;
    public Stat speed;

    public Text SoulsUI;
    public ParticleSystem hitVFX;

    public GameObject deathUI;

    public GameObject player;
    
    
    
    void Start ()
    {
        
        if(gameObject.tag == "Player"){
            player = gameObject;
        }
        else{
            player = GameObject.FindGameObjectWithTag("Player");
            coins = Random.Range(5, 25);
        }
        
        currentHealth = maxHealth;
    }

    void Update() {
        if (!isDying && currentHealth <= 0) {
            Die();
        }
    }

    public void TakeDamage (float damage) {
        hitVFX.Play();
        //armor system
        damage -= armor.GetValue();
        damage = Mathf.Clamp(damage, 0, float.MaxValue);

        currentHealth -= damage;

        if (gameObject.tag == "Enemy") {
            StartCoroutine(stunEnemy());
        }
        if(gameObject.tag == "Player") {
            gameObject.GetComponent<PlayerController>().Stun();
        }
    }

    
    public virtual void Die () {
        isDying = true;
        Debug.Log("I died!");
        if (FindObjectOfType<LockableTargets>()._possibleTargets.Contains(this.gameObject)) {
            FindObjectOfType<LockTarget>().DelockTarget();
        }
        if (FindObjectOfType<SwordCollision>().immuneEnemies.Contains(this.gameObject)) {
            FindObjectOfType<SwordCollision>().immuneEnemies.Remove(this.gameObject);
        }
        //Die in some way
        if (gameObject.tag == "Enemy") {
            player.GetComponent<PlayerStats>().coins += coins;
            player.GetComponent<CurseMeter>().curseMeter += (float)coins / player.GetComponent<CurseMeter>().fillRate;
            gameObject.GetComponent<Enemy_Controller>().shrine.GetComponent<AI_Manager>().enemiesReadyToAttack.Remove(gameObject);
            gameObject.GetComponent<Enemy_Controller>().shrine.GetComponent<AI_Manager>().surroundSpotAvailability[gameObject.GetComponent<Enemy_Controller>().surroundIndex] = true;
            Destroy(this.gameObject, 0.025f);
        }
        if (this.gameObject.tag == "Player"){
            
            StartCoroutine(PlayerDeath(this.gameObject));
        }
        
    }
    public IEnumerator stunEnemy()
    {
        GetComponent<Enemy_Controller>().stunned = true;
        yield return new WaitForSeconds(0.35f);
        GetComponent<Enemy_Controller>().stunned = false;
    }

    public IEnumerator PlayerDeath(GameObject player){
        
        // disable player move script
        // play death animation
        deathUI.SetActive(true);
        player.GetComponent<CharacterController>().enabled = false;
        Transform[] springTransforms = FindObjectOfType<PlayerStats>().SpringTransforms;
        Vector3 respawnPosition = Vector3.zero;
        float minMagnitude = float.MaxValue;
        for(int i = 0; i < springTransforms.Length; i ++){
            float mag = (player.transform.position - springTransforms[i].transform.position).magnitude;
            if(mag < minMagnitude){
                minMagnitude = mag;
                respawnPosition = springTransforms[i].position;
            }
        }
        yield return new WaitForSeconds(3f);
        player.transform.position = respawnPosition;
        player.GetComponent<PlayerStats>().currentHealth = player.GetComponent<PlayerStats>().maxHealth;
        player.GetComponent<CharacterController>().enabled = true;
        yield return null;
    }
}
