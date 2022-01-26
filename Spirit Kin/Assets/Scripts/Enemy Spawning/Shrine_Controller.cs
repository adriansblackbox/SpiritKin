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


    public bool checkValidity(int quadrant)
    {
        
        //check if quadrant has > 50% of enemies if so return false
        var enemiesContainer = transform.GetChild(0);
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
            Debug.Log("Shrine at (" + transform.position.x + ", " + transform.position.z + ") has " + enemiesContainer.childCount + " Enemies");
            Debug.Log("Shrine at (" + transform.position.x + ", " + transform.position.z + ") has " + numberOfEnemiesInSelectedQuadrant + " Enemies in Quadrant #" + quadrant);
            if (numberOfEnemiesInSelectedQuadrant / (float) enemiesContainer.childCount > 0.5f)
                return false;
            else  
                return true;
        }
        return true;
    }
}
