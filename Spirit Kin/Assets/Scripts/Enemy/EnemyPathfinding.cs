using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPathfinding : MonoBehaviour
{
    public Vector3 spawnLocation;
    public GameObject player;
    public GameObject eneME;
    
    public NavMeshAgent enemyMe;
    public NavMeshPath path;

    // Start is called before the first frame update
    void Start()
    {
        path = new NavMeshPath();
        player = GameObject.FindWithTag("Player"); 
        spawnLocation = eneME.transform.position;
        enemyMe = eneME.GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        enemyMe.CalculatePath(player.transform.position, path);
        if(path.status == NavMeshPathStatus.PathComplete) { // Check if player is in navmesh. Has something to do with the NavMeshPathStatus enum
            enemyMe.SetDestination(player.transform.position);
        }
        else{
            enemyMe.SetDestination(spawnLocation);
            // Player not close enough. Patrol behavior.
            // Consider adding rules for if player tries to exploit AI by attacking them from outside the navmesh range
        } 
    }
}
