using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
public class PauseMenu : MonoBehaviour
{
    // Start is called before the first frame update
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    public GameObject Player;

    public GameObject PauseFirstButton;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)){
            if(GameIsPaused){
                Cursor.lockState = CursorLockMode.Locked;
                // Player.GetComponent<PlayerController>().enabled = true;
                // Player.GetComponent<PlayerCombat>().enabled = true;
                
                Resume();
            }else{
                Cursor.lockState = CursorLockMode.None;
                // Player.GetComponent<PlayerController>().enabled = false;
                // Player.GetComponent<PlayerCombat>().enabled = false;
                Pause();
            }
        }
    }

    public void Resume (){
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }
    public void Pause (){
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        //clear selected button
        EventSystem.current.SetSelectedGameObject(null);
        //reassign
        EventSystem.current.SetSelectedGameObject(PauseFirstButton);
    }

    public void LoadMenu(){
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
    public void Quit(){
        Debug.Log("Game Quit");
        Application.Quit();
    }
}
