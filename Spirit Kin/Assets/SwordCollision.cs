using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordCollision : MonoBehaviour
{
    public Transform BladeRayOrigin;
    public float BladeLength = 10f;
    public LayerMask layerMask;
    private void Update() {
        RaycastHit hit;
        if (Physics.Raycast(BladeRayOrigin.position, BladeRayOrigin.TransformDirection(Vector3.left), out hit, BladeLength, layerMask))
        {
            //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            Debug.DrawRay(BladeRayOrigin.position, BladeRayOrigin.TransformDirection(Vector3.left) * BladeLength, Color.red);
            Debug.Log("Did Hit");
            hit.transform.gameObject.GetComponent<CharacterStats>().TakeDamage(20);
        }
        else
        {
            Debug.DrawRay(BladeRayOrigin.position, BladeRayOrigin.TransformDirection(Vector3.left) * BladeLength, Color.yellow);
        }
    }
}
