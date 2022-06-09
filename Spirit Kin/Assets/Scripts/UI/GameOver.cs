using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public GameObject GameOverFirstButton;

    public GameObject GameOverUI;

    public AudioSource sounds;

    public AudioClip gameOverSound;

    public PlayfabManager playfabManager;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LoadGameOver()
    {
        //update leaderboard
        // playfabManager.SendLeaderboard(100);

        sounds.PlayOneShot (gameOverSound);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        GameOverUI.SetActive(true);
        Time.timeScale = 0f;
        EventSystem.current.SetSelectedGameObject(null);

        //reassign
        EventSystem.current.SetSelectedGameObject (GameOverFirstButton);
    }

    public void Restart()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        GameOverUI.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        GameOverUI.SetActive(false);
        SceneManager.LoadScene("MainMenu");
    }
}
