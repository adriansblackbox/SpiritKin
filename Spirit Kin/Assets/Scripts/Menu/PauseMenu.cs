using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
public class PauseMenu : MonoBehaviour
{
    // Start is called before the first frame update
    public bool GameIsPaused = false;
    public TeaShop teaShopMenu;
    public GameObject pauseMenuUI;
    public GameObject Player;

    public GameObject PauseFirstButton;
    public GameObject MouseKeyboardControls;
    public GameObject XboxControllerControls;
    void Start()
    {
        teaShopMenu = GameObject.Find("TeaShop").GetComponent<TeaShop>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Start Button")){
            if(GameIsPaused){
                // Player.GetComponent<PlayerController>().enabled = true;
                // Player.GetComponent<PlayerCombat>().enabled = true;
                
                Resume();
            }else{
                // Player.GetComponent<PlayerController>().enabled = false;
                // Player.GetComponent<PlayerCombat>().enabled = false;
                Pause();
            }
        }
    }

    public void Resume (){
        if(!teaShopMenu.isOpen){
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }
    public void Pause (){
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        //clear selected button
        EventSystem.current.SetSelectedGameObject(null);
        //reassign
        EventSystem.current.SetSelectedGameObject(PauseFirstButton);
    }

    public void Restart(){
        // Time.timeScale = 1f;
        SceneManager.LoadScene("Main Scene");
    }
    public void LoadMenu(){
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
    public void Quit(){
        Debug.Log("Game Quit");
        Application.Quit();
    }
    public void toggleControls()
    {
        MouseKeyboardControls.SetActive(!MouseKeyboardControls.activeSelf);
        XboxControllerControls.SetActive(!XboxControllerControls.activeSelf);
    }
}
