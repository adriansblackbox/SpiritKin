using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioClip waterSound;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 1f * Time.deltaTime);
    }
}
