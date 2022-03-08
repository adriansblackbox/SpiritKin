using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStats : MonoBehaviour
{
    public int coins;
    public float maxHealth = 100;
    public float currentHealth;

    public Stat armor;
    public Stat damage;
    public Stat speed;

    public Text SoulsUI;
    public ParticleSystem hitVFX;

    public GameObject deathScene;
    public GameObject player;
    
    void Start ()
    {
        player = GameObject.Find("Player");
        currentHealth = maxHealth;
        coins = 20;
    }

    void Update() {
    }

    public void TakeDamage (float damage) {
        hitVFX.Play();
        //armor system
        damage -= armor.GetValue();
        damage = Mathf.Clamp(damage, 0, float.MaxValue);

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
            player.GetComponent<PlayerStats>().coins += coins;
            gameObject.GetComponent<Enemy_Controller>().shrine.GetComponent<AI_Manager>().enemiesReadyToAttack.Remove(gameObject);
            Destroy(this.gameObject);
        }
        if (FindObjectOfType<LockableTargets>()._possibleTargets.Contains(this.gameObject)) {
            FindObjectOfType<LockTarget>().DelockTarget();
        }
        if (FindObjectOfType<SwordCollision>().immuneEnemies.Contains(this.gameObject)) {
            FindObjectOfType<SwordCollision>().immuneEnemies.Remove(this.gameObject);
        }
        if (gameObject.tag == "Player"){
            StartCoroutine(PlayerDeath(gameObject.transform));
        }
        
    }
    public IEnumerator PlayerDeath(Transform playerTransform){
        //deathScene.SetActive(true);
        // disable player move script
        // play death animation
        //
        Transform[] springTransforms = FindObjectOfType<PlayerStats>().SpringTransforms;
        Vector3 respawnPosition = Vector3.zero;
        float minMagnitude = float.MaxValue;
        for(int i = 0; i < springTransforms.Length; i ++){
            float mag = (playerTransform.position - springTransforms[i].transform.position).magnitude;
            if(mag < minMagnitude){
                minMagnitude = mag;
                respawnPosition = springTransforms[i].position;
            }
        }
        Debug.Log(respawnPosition);
        playerTransform.position = respawnPosition;
        yield return null;
    }
}
