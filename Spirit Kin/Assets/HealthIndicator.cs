using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthIndicator : MonoBehaviour
{
    public Image healthCircle;
    public float health = 0;
    public float maxHealth = 100;

    void Update()
    {
        adjustHealth();
    }

    //-100 y is 0 health
    //0 y is 100 health
    public void adjustHealth()
    {
        healthCircle.fillAmount = health/maxHealth;
    }
}
