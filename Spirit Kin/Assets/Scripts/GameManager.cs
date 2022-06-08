using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public bool gameOver;
    private bool showingGameOver;
    public Shrine fullyCursedShrine;
    
    [SerializeField] Text gameOverText;

    [SerializeField] Camera lanternGameOverCamera;
    [SerializeField] Camera mountainGameOverCamera;
    [SerializeField] Camera bambooGameOverCamera;
    [SerializeField] Camera statueGameOverCamera;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            gameOver = true;

        if (gameOver && !showingGameOver)
        {
            showingGameOver = true;
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
                lanternGameOverCamera.GetComponent<CameraFade>().Reset();
                gameOverText.text = "Lantern Shrine Has Been Cursed";
                break;
            case "Mountain Shrine":
                mountainGameOverCamera.enabled = true;
                mountainGameOverCamera.GetComponent<CameraFade>().Reset();
                gameOverText.text = "Mountain Shrine Has Been Cursed";
                break;
            case "Bamboo Shrine":
                bambooGameOverCamera.enabled = true;
                bambooGameOverCamera.GetComponent<CameraFade>().Reset();
                gameOverText.text = "Bamboo Shrine Has Been Cursed";
                break;
            case "Statue Shrine":
                statueGameOverCamera.enabled = true;
                statueGameOverCamera.GetComponent<CameraFade>().Reset();
                gameOverText.text = "Statue Shrine Has Been Cursed";
                break;
        }

        for (int i = 0; i < 10; i++)
        {
            FindObjectOfType<Enemy_Spawner>().spawnEnemy(fullyCursedShrine.gameObject);
            yield return new WaitForSeconds(0.25f);
        }

        FindObjectOfType<GameOver>().LoadGameOver();

        yield return null;
    }
}
