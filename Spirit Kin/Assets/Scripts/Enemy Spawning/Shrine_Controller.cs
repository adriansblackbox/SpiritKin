using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shrine_Controller : MonoBehaviour
{

    //SEPARATE SCRIPT (SHRINE CONTROLLER)
        //Split shrines into 4 quadrants
            //Each quadrant can hold at most 1/2 of total enemies
                //if a quadrant has more than 1/2 of total enemies select enough enemies at random to where its below 1/2
                    //if enemy is not current chasing player change the randomly selected enemies to relocating state
                        //send them to a neighboring quadrant for ease of use    

    private Transform enemiesContainer;

    private void Start() 
    {
        enemiesContainer = transform.GetChild(0);
    }


    public bool checkIfNeedRelocate(int quadrant)
    {
        
        //check if quadrant has > 50% of enemies if so return false
        if (enemiesContainer.childCount >= 4) {
            float numberOfEnemiesInSelectedQuadrant = 0;
            for (int i = 0; i < enemiesContainer.childCount; i++)
            {
                if (enemiesContainer.GetChild(i).GetComponent<Enemy_Controller>().getQuadrant() == quadrant)
                    numberOfEnemiesInSelectedQuadrant+=1;
            }
            //CHECK DIFFICULTY INDEX WHICH WE DETERMINE THROUGH PLAYTESTING & IF NUMBER OF ENEMIES IN QUADRANT BELOW THAT INDEX
                //RETURN TRUE
            //ELSE 
                //RETURN FALSE
            if (numberOfEnemiesInSelectedQuadrant / (float) enemiesContainer.childCount > 0.5f)
                return true;
            else  
                return false;
        }
        return true;
    }

    public float checkPatrol(float baseChance)
    {
        //go through enemies
            //if > 50% are patroling special chance to go idle
            //else normal chance
        var enemyCount = enemiesContainer.childCount;
        float enemiesPatrolingCount = 0.0f;
        for (int i = 0; i < enemyCount; i++)
        {
            if (enemiesContainer.GetChild(i).GetComponent<Enemy_Controller>().EnemyMotion == Enemy_Controller.MotionState.Patroling) enemiesPatrolingCount += 1;
        }
        if (enemiesPatrolingCount / (float) enemyCount > 0.5f)
            return baseChance * 1.5f;
        return baseChance;
    }

    public float checkIdle(float baseChance)
    {
        //go through enemies
            //if < .25f are patroling special chance to go idle
            //else normal chance
        var enemyCount = enemiesContainer.childCount;
        float enemiesPatrolingCount = 0.0f;
        for (int i = 0; i < enemyCount; i++)
        {
            if (enemiesContainer.GetChild(i).GetComponent<Enemy_Controller>().EnemyMotion == Enemy_Controller.MotionState.Patroling) enemiesPatrolingCount += 1;
        }
        if (enemiesPatrolingCount / (float) enemyCount < 0.4f)
            return baseChance * 1.5f;
        else
            return baseChance;
    }
}
