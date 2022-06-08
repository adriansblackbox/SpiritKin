using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class PauseMenu : MonoBehaviour
{
    // Start is called before the first frame update
    public MainMenu MainMenu;
    public bool GameIsPaused = false;
    public TeaShop teaShopMenu;
    public equipmentShop equipMenu;
    public GameObject pauseMenuUI;
    public GameObject Player;

    public GameObject PauseFirstButton;
    public GameObject ControlOverlay;
    public GameObject ToggleIndicatorOn;
    public GameObject ToggleIndicatorOff;

    [SerializeField] GameManager gm;
    void Start()
    {
        teaShopMenu = GameObject.Find("TeaShop").GetComponent<TeaShop>();
        equipMenu = GameObject.Find("EquipmentShop").GetComponent<equipmentShop>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Start Button")){
            if(GameIsPaused){
                Resume();
            }else if(teaShopMenu.isOpen || equipMenu.isOpen){
                teaShopMenu.CloseMenu();
                equipMenu.CloseMenu();
                
                
            }else{
                if (!gm.gameOver)
                    Pause();
            }
        }
    }

    public void Resume (){
        if(!teaShopMenu.isOpen){
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        Player.GetComponent<PlayerController>().enabled = true;
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }
    public void Pause (){
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Player.GetComponent<PlayerController>().enabled = false;
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
        ToggleIndicatorOn.SetActive(!ToggleIndicatorOn.activeSelf);
        ToggleIndicatorOff.SetActive(!ToggleIndicatorOff.activeSelf);
        ControlOverlay.SetActive(!ControlOverlay.activeSelf);
    }
    public void GraphicsApply(){
        MainMenu.GraphicsApply();
    }
}
