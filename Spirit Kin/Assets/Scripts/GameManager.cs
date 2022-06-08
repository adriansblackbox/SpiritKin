using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool gameOver;
    public Shrine fullyCursedShrine;

    [SerializeField] Camera lanternGameOverCamera;
    [SerializeField] Camera mountainGameOverCamera;
    [SerializeField] Camera bambooGameOverCamera;
    [SerializeField] Camera statueGameOverCamera;

    private void Update()
    {
        if (gameOver)
        {
            gameOver = false;
            StartCoroutine(gameOverCutscene());
        }
    }

    IEnumerator gameOverCutscene() //need some type of shrine identifier
    {
        FindObjectOfType<ControlOverlayHandler>().gameObject.SetActive(false); //hide player UI
        FindObjectOfType<PlayerController>().enabled = false;
        FindObjectOfType<PlayerStats>().enabled = false;

        switch (fullyCursedShrine.gameObject.name)
        {
            case "Lantern Shrine":
                lanternGameOverCamera.enabled = true;
                break;
            case "Mountain Shrine":
                mountainGameOverCamera.enabled = true;
                break;
            case "Bamboo Shrine":
                bambooGameOverCamera.enabled = true;
                break;
            case "Statue Shrine":
                statueGameOverCamera.enabled = true;
                break;
        }

        

        yield return new WaitForSeconds(2f);

        FindObjectOfType<GameOver>().LoadGameOver();

        yield return null;
    }
}
