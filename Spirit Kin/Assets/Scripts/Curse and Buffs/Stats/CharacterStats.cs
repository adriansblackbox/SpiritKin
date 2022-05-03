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

    public int fillAmount;

    public Stat armor;
    public Stat damage;
    public Stat speed;

    public Text SoulsUI;
    public ParticleSystem hitVFX;

    public GameObject deathUI;
    public GameObject healthBarCanvas;
    public GameObject healthBar;

    public GameObject player;
    
    
    
    void Start ()
    {
        
        if (gameObject.tag == "Player")
        {
            player = gameObject;
        }
        else
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        currentHealth = maxHealth;
    }

    void Update() {
        if (!isDying && currentHealth <= 0) {
            Die();
        }
    }

    public void TakeDamage (float damage, float knockBackStrength) {
        hitVFX.Play();
        //armor system
        damage -= armor.GetValue();
        damage = Mathf.Clamp(damage, 0, float.MaxValue);

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, float.MaxValue);
        
        if (gameObject.tag != "Player" && currentHealth != maxHealth && currentHealth > 0) healthBarCanvas.SetActive(true);
        if (gameObject.tag != "Player") healthBar.transform.localScale = new Vector3 (40 * (currentHealth / maxHealth), 0.3f, 1f);
        //healthBarCanvas.GetComponent<EnemyHealthBar>().takeDamageUI(currentHealth / maxHealth); // Lerp health bar aint doin it rn. Swap out one-line logic with this.

        //vvv will reimplement once its cleaner
        if (gameObject.tag == "Enemy" && currentHealth > 0)
        {
            Enemy_Controller enemyController = gameObject.GetComponent<Enemy_Controller>();
            if (enemyController.EnemyAttack != Enemy_Controller.AttackState.Attacking)
            {
                enemyController.GenerateKnockBack(knockBackStrength);
                if (enemyController.EnemyMotion == Enemy_Controller.MotionState.Stunned)
                {
                    enemyController.changeStun();
                    enemyController.resetKnockback();
                    enemyController.GenerateKnockBack(knockBackStrength);
                }
                else
                {
                    enemyController.beginStun();
                }
            }         
        }
        if (gameObject.tag == "Player") 
        {
            if(!gameObject.GetComponent<Animator>().GetBool("Death"))
            gameObject.GetComponent<PlayerController>().Stun();
        }
    }

    
    public virtual void Die () {
        isDying = true;
        Debug.Log("I died!");
        if (FindObjectOfType<LockableTargets>()._possibleTargets.Contains(this.gameObject)) {
            FindObjectOfType<LockTarget>().Target = null;
        }
        if (FindObjectOfType<SwordCollision>().immuneEnemies.Contains(this.gameObject)) {
            FindObjectOfType<SwordCollision>().immuneEnemies.Remove(this.gameObject);
        }

        if (gameObject.tag == "Enemy") {
            gameObject.GetComponent<Enemy_Controller>().enemyAnimator.SetBool("Dead", isDying);
            gameObject.GetComponent<Enemy_Controller>().shrine.GetComponent<AI_Manager>().enemiesReadyToAttack.Remove(gameObject);
            gameObject.GetComponent<Enemy_Controller>().enemyCollider.isTrigger = true;
            gameObject.GetComponent<Enemy_Controller>().changeState(Enemy_Controller.MotionState.Waiting);
            gameObject.GetComponent<Enemy_Controller>().EnemyAttack = Enemy_Controller.AttackState.Waiting;
            gameObject.GetComponent<Enemy_Controller>().resetSurround();
            if (currentHealth != maxHealth) healthBarCanvas.SetActive(false);
        }
        
        if (this.gameObject.tag == "Player") {
            
            StartCoroutine(PlayerDeath(this.gameObject));
        }
        
    }

    public IEnumerator PlayerDeath(GameObject player) {
        
        // disable player move script
        // play death animation
        deathUI.SetActive(true);
        player.GetComponent<Animator>().SetBool("Death", true);
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
        player.GetComponent<Animator>().SetBool("Death", false);
        player.transform.position = respawnPosition;
        player.GetComponent<CharacterController>().enabled = true;
        player.GetComponent<PlayerStats>().currentHealth = player.GetComponent<PlayerStats>().maxHealth;
        yield return null;
    }
}
