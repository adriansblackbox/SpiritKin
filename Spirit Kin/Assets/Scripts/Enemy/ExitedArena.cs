using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitedArena : MonoBehaviour
{
    [SerializeField] Shrine shrine;

    private void OnTriggerExit(Collider col) 
    {
        if (col.tag == "Enemy")
        {
            col.transform.parent.GetComponent<Enemy_Controller>().exitedArena = true;
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            FindObjectOfType<Enemy_Spawner>().shrinePlayerIsAt = shrine;
        }
    }
}
