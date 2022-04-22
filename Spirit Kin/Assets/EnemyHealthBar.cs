using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField]
    private CharacterStats enemy_Stats;
    public GameObject healthBar;
    public GameObject camera;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.transform.localScale = new Vector3 (7 * (enemy_Stats.currentHealth / enemy_Stats.maxHealth), 0.3f, 1f);
    }
}
