using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPathfinding : MonoBehaviour
{
    public GameObject player;
    public GameObject eneME;
    
    public NavMeshAgent enemyMe;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player"); 
        enemyMe = eneME.GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("coom");
         if(true) { // Check if player is in navmesh. Has something to do with the NavMeshPathStatus enum
            Debug.Log("BIG COOM");
            enemyMe.SetDestination(player.transform.position);
            Debug.Log("NUTTE");
        } 
    }
}
