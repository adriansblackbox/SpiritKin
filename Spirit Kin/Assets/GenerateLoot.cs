using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateLoot : MonoBehaviour
{
    public GameObject thisObj;
    public GameObject myPrefab;
    public GameObject thePlayer;
    // Start is called before the first frame update
    void Start()
    {
       thePlayer = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E)){
            GameObject newOne = Instantiate(myPrefab, transform.position, Quaternion.identity);
            newOne.GetComponent<follow>().Target = thePlayer.transform;
            Destroy(thisObj);
        }
    }
}
