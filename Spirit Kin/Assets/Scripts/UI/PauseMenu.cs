using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class PauseMenu : MonoBehaviour
{

    [Header("Volume Settings")]
    [SerializeField] private Slider masterVolumeSlider = null;
    [SerializeField] private float defaultVolume = 0.5f;

    [Header("Gameplay Settings")]
    [SerializeField] private Slider controllerSenSlider = null;
    [SerializeField] private int defaultSen = 5;
    public int mainControllerSen = 5;

    [Header("Toggle Settings")]
    [SerializeField] private Toggle invertYToggle = null;
    [SerializeField] Toggle ControlIndicator;

    [Header("Graphics Settings")]
    [SerializeField] private Slider brightnessSlider = null;
    [SerializeField] private int defaultBrightness = 4;

    [Space(10)]
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private Toggle fullScreenToggle;

    [Header("Resolution DropDown")]
    public TMP_Dropdown resolutionDropdown;
    private Resolution[] resolutions;

    // Start is called before the first frame update
    public MainMenu MainMenu;
    public bool GameIsPaused = false;
    public TeaShop teaShopMenu;
    public equipmentShop equipMenu;
    public GameObject pauseMenuUI;
    public GameObject Player;
    [SerializeField] GameObject darkenBackground;
    
    [SerializeField] GameObject UICanvas;

    public GameObject PauseFirstButton;
    public GameObject ControlOverlay;

    private int _qualityLevel;
    private bool _isFullscreen;
    private float _brightnessLevel;

    [SerializeField] GameManager gm;

    [SerializeField] GameObject settingsMenu;
    
    void Start()
    {
        teaShopMenu = GameObject.Find("TeaShop").GetComponent<TeaShop>();
        equipMenu = GameObject.Find("EquipmentShop").GetComponent<equipmentShop>();

        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        
        for(int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            if (!options.Contains(option))
                options.Add(option);

            if(resolutions[i].width == Screen.width && resolutions[i].height == Screen.height){
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        // resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Start Button")){
            if(GameIsPaused && !settingsMenu.activeSelf){
                Resume();
            }else if(teaShopMenu.isOpen || equipMenu.isOpen){
                teaShopMenu.CloseMenu();
                equipMenu.CloseMenu();
            }else{
                if (!gm.gameOver && !FindObjectOfType<TutorialManager>().tutorialOn)
                    Pause();
            }
        }
    }

    public void Resume (){
        if(!teaShopMenu.isOpen){
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        darkenBackground.SetActive(false);
        Player.GetComponent<PlayerController>().enabled = true;
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        UICanvas.SetActive(true);
    }
    public void Pause (){
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Player.GetComponent<PlayerController>().enabled = false;
        pauseMenuUI.SetActive(true);
        darkenBackground.SetActive(true);
        Time.timeScale = 0.0f;
        GameIsPaused = true;
        SetButton(PauseFirstButton);
        UICanvas.SetActive(false);
    }

    public void SetButton(GameObject buttonToSet)
    {
        //clear selected button
        EventSystem.current.SetSelectedGameObject(null);
        //reassign
        EventSystem.current.SetSelectedGameObject(buttonToSet);
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
        if (ControlIndicator.isOn)
        {
            ControlOverlay.SetActive(false);
        }
        else
        {
            ControlOverlay.SetActive(true);
        }
        
    }

    public void SetResolution(int resolutionIndex){
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetBrightness(float brightness){
        _brightnessLevel = brightness;
    }

    public void SetFullScreen(bool isFullscreen){
        _isFullscreen = isFullscreen;
    }

    public void SetQuality(int qualityIndex){
        _qualityLevel = qualityIndex;
    }

    public void SetMasterVolume (float value){
        AudioListener.volume = value;
    }

    public void VolumeApply(){
        PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
    }

    public void SetControllerSen(float sensitivity){
        mainControllerSen = Mathf.RoundToInt(sensitivity);
    }

    public void GameplayApply(){

        PlayerPrefs.SetFloat("masterSen", mainControllerSen);
    }

    public void ResetButton(string MenuType){

        if (MenuType == "Graphics"){
            //reset brightness value
            brightnessSlider.value = defaultBrightness;

            qualityDropdown.value = 1;
            QualitySettings.SetQualityLevel(1);

            fullScreenToggle.isOn = false;
            Screen.fullScreen = false;

            Resolution currentResolution = Screen.currentResolution;
            Screen.SetResolution(currentResolution.width, currentResolution.height, Screen.fullScreen);
            resolutionDropdown.value = 0;
            GraphicsApply();
        }

        if (MenuType == "Audio"){
            AudioListener.volume = defaultVolume;
            masterVolumeSlider.value = defaultVolume;
            VolumeApply();
        }

        if (MenuType == "Gameplay"){
            controllerSenSlider.value = defaultSen;
            mainControllerSen = defaultSen;
            GameplayApply();
        }

        if (MenuType == "Controls")
        {
            ControlOverlay.SetActive(true);
            ControlIndicator.isOn = false;
        }
    }

    public void GraphicsApply()
    {
        PlayerPrefs.SetFloat("masterBrightness", _brightnessLevel);
        //change your brightness with your post processing or whatever it is

        PlayerPrefs.SetInt("masterQuality", _qualityLevel);
        QualitySettings.SetQualityLevel(_qualityLevel);

        PlayerPrefs.SetInt("masterFullscreen", (_isFullscreen ? 1:0));
        Screen.fullScreen = _isFullscreen;
    }
}
