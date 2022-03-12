using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitedArena : MonoBehaviour
{
    private void OnTriggerExit(Collider col) {
        if (col.tag == "Enemy")
        {
            col.transform.GetComponent<Enemy_Controller>().exitedArena = true;
        }
    }
}
