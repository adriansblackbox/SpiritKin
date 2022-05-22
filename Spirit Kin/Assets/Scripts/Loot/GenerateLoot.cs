using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateLoot : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> loot = new List<GameObject>();

    [SerializeField]
    [Range(1, 99)]
    private int minNumber = 7;

    [Range(2, 100)]
    private int maxNumber = 20;

    [SerializeField]
    private Transform spawnPoint;

    private bool hasBeenCollected = false;

    [SerializeField]
    private bool spawnLoot = false;

    private GameObject[] totalCoins;

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
        
        if(Input.GetKeyDown(KeyCode.E)){
            spawnLoot = true;
        }

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
        StartCoroutine(CreateLoot(numberOfLoot));
        
    }

    IEnumerator CreateLoot(int numberOfLoot)
    {
        for (int i = 0; i < numberOfLoot; i++)
        {
            GameObject tempLoot =
                Instantiate(loot[Random.Range(0, loot.Count)],
                spawnPoint.position,
                Quaternion.identity);
        }
        yield return new WaitForSeconds(0.15f);
    }
}
