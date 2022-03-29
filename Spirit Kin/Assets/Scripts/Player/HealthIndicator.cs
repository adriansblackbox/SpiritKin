using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthIndicator : MonoBehaviour
{
    //.81 start to move mask
    //.19 mask missing

    //top end is 0.81 health circle = 1.00 mask

    //bottom end is 0.19 health circle = 0.00 mask

    public Image healthCircle;
    public GameObject player;

    void Update()
    {
        adjustHealth();
    }

    public void adjustHealth()
    {
        healthCircle.fillAmount = player.GetComponent<PlayerStats>().currentHealth/player.GetComponent<PlayerStats>().maxHealth;
    }
}
