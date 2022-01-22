using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Controller : MonoBehaviour
{
    //Waypoints (4-8 waypoints around the shrine)
        //more will ensure less obvious patrol paths
            //can generate paths rather than having them set

//SEPARATE SCRIPT (SHRINE CONTROLLER)
    //Split shrines into 4 quadrants
        //Each quadrant can hold at most 1/2 of total enemies
            //if a quadrant has more than 1/2 of total enemies select enough enemies at random to where its below 1/2
                //if enemy is not current chasing player change the randomly selected enemies to relocating state

    public enum MotionState {
        Patroling,
        Idling,
        Alerted,
        Chasing,
        Returning,
        Relocating
    }

    //Enemies will surround player
        //They will attack once at a time

    public enum AttackState {
        Attacking,
        Surrounding,
        NotAttacking,
    }

    public MotionState EnemyMotion;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (EnemyMotion)
        {
            case MotionState.Patroling:
                break;
            case MotionState.Idling:
                break;
            case MotionState.Alerted:
                break;
            case MotionState.Chasing:
                break;
            case MotionState.Returning:
                break;
            default:
                break;
        }
    }
}
