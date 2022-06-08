using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    [SerializeField] private float lifeTime = 2f;

    // Update is called once per frame
    void Update()
    {
        if(lifeTime > 0)
            lifeTime -= Time.deltaTime;
        else
        Destroy(this.gameObject);
        
    }
}
