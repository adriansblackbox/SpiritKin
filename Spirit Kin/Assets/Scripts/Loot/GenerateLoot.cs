using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateLoot : MonoBehaviour
{
    [SerializeField]
    private GameObject loot;

    [SerializeField]
    [Range(1, 99)]
    private int minNumber = 7;

    [SerializeField]
    [Range(2, 100)]
    private int maxNumber = 20;

    [SerializeField]
    private Transform spawnPoint;

    [SerializeField]
    public bool hasBeenCollected = false;

    [SerializeField]
    public bool spawnLoot = false;

    private void OnValidate()
    {
        if (minNumber > maxNumber)
        {
            maxNumber = minNumber + 1;
        }
    }

    void Awake(){

    }

    void Update()
    {
        
        // if (Input.GetKeyDown(KeyCode.E))
        // {
        //     spawnLoot = true;
        // }

        if (spawnLoot && !hasBeenCollected)
        {
            spawnLoot = false;

            Loot();
        }
    }

    private void Loot()
    {
        hasBeenCollected = true;
        int numberOfLoot = Random.Range(minNumber, maxNumber);
        Debug.Log(numberOfLoot);
        StartCoroutine(CreateLoot(numberOfLoot));
        
    }

    IEnumerator CreateLoot(int numberOfLoot)
    {
        for (int i = 0; i < numberOfLoot; i++)
        {
            GameObject tempLoot =
                Instantiate(loot,
                spawnPoint.position,
                Quaternion.identity);
            
            yield return new WaitForSeconds(0.05f);
        }
    }
}
