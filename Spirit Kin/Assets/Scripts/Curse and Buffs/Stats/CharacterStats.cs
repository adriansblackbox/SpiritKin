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

    public virtual void TakeDamage (float damage, float knockBackStrength) {
        hitVFX.Play();
        //armor system
        damage -= armor.GetValue();
        damage = Mathf.Clamp(damage, 0.05f * currentHealth, float.MaxValue);

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, float.MaxValue);
        
        if (currentHealth != maxHealth && currentHealth > 0) healthBarCanvas.SetActive(true);
        healthBar.transform.localScale = new Vector3 (40 * (currentHealth / maxHealth), 0.3f, 1f);
        //healthBarCanvas.GetComponent<EnemyHealthBar>().takeDamageUI(currentHealth / maxHealth); // Lerp health bar aint doin it rn. Swap out one-line logic with this.

        //vvv will reimplement once its cleaner
        if (currentHealth > 0)
        {
            Enemy_Controller enemyController = gameObject.GetComponent<Enemy_Controller>();
            if (!enemyController.immuneToStun)
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
    }

    public virtual void Die () {
        isDying = true;
        Debug.Log("Enemy died!");
        gameObject.layer = 12;
        if (FindObjectOfType<SwordCollision>().immuneEnemies.Contains(this.gameObject))
            FindObjectOfType<SwordCollision>().immuneEnemies.Remove(this.gameObject);
        if (FindObjectOfType<LockTarget>().Target = this.gameObject.transform)
            FindObjectOfType<LockTarget>().DelockTarget();

        Enemy_Controller eco = gameObject.GetComponent<Enemy_Controller>();
        eco.LockOnArrow.GetComponent<LockOnArrow>().DestoryArrow();    
        eco.enemyAnimator.SetBool("Dead", isDying);
        eco.shrine.GetComponent<AI_Manager>().enemiesReadyToAttack.Remove(gameObject);
        eco.enemyCollider.isTrigger = true;
        eco.changeState(Enemy_Controller.MotionState.Waiting);
        eco.EnemyAttack = Enemy_Controller.AttackState.Waiting;
        eco.resetSurround();
        healthBarCanvas.SetActive(false);
    }
}
