using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitedArena : MonoBehaviour
{
    private void OnTriggerExit(Collider col) {
        Debug.Log("OnTriggerExit Triggered " + gameObject.name + ": (" + col.name + ")");
        if (col.tag == "Enemy")
        {
            Debug.Log("Set Enemy's exitedArena bool to true");
            col.transform.parent.GetComponent<Enemy_Controller>().exitedArena = true;
        }
    }
}
