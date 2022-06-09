using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [Header("Volume Settings")]
    [SerializeField]
    private Slider masterVolumeSlider = null;

    [SerializeField]
    private float defaultVolume = 0.5f;

    [Header("Gameplay Settings")]
    [SerializeField]
    private Slider controllerSenSlider = null;

    [SerializeField]
    private int defaultSen = 5;

    public int mainControllerSen = 5;

    [Header("Toggle Settings")]
    [SerializeField]
    private Toggle invertYToggle = null;

    [SerializeField]
    Toggle ControlIndicator;

    [Header("Graphics Settings")]
    [SerializeField]
    private Slider brightnessSlider = null;

    [SerializeField]
    private int defaultBrightness = 4;

    [Space(10)]
    [SerializeField]
    private TMP_Dropdown qualityDropdown;

    [SerializeField]
    private Toggle fullScreenToggle;

    [Header("Resolution DropDown")]
    public TMP_Dropdown resolutionDropdown;

    private Resolution[] resolutions;

    [Header("First Buttons")]

    [SerializeField]
    private GameObject settingsFirstButton;
    [SerializeField]
    private GameObject gameover;
    [SerializeField]
    private GameObject GameOverFirstButton;

    [SerializeField]
    private bool buttonSet = false;

    public bool GameIsPaused = false;

    public TeaShop teaShopMenu;

    public equipmentShop equipMenu;

    public GameObject pauseMenuUI;

    public GameObject Player;

    [SerializeField]
    GameObject darkenBackground;

    [SerializeField]
    GameObject UICanvas;

    public GameObject PauseFirstButton;

    public GameObject ControlOverlay;

    private int _qualityLevel;

    private bool _isFullscreen;

    private float _brightnessLevel;

    [SerializeField]
    GameManager gm;

    [SerializeField]
    GameObject settingsMenu;

    public bool keyboard = false;
    void Start()
    {
        teaShopMenu = GameObject.Find("TeaShop").GetComponent<TeaShop>();
        equipMenu =
            GameObject.Find("EquipmentShop").GetComponent<equipmentShop>();

        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option =
                resolutions[i].width + " x " + resolutions[i].height;
            if (!options.Contains(option)) options.Add(option);

            if (
                resolutions[i].width == Screen.width &&
                resolutions[i].height == Screen.height
            )
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions (options);

        // resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    // Update is called once per frame
    void Update()
    {
        if (
            Input.GetKey(KeyCode.JoystickButton0) || Input.GetKey(KeyCode.JoystickButton1) || Input.GetKey(KeyCode.JoystickButton2) || 
            Input.GetKey(KeyCode.JoystickButton3) || Input.GetKey(KeyCode.JoystickButton4) || Input.GetKey(KeyCode.JoystickButton5) || 
            Input.GetKey(KeyCode.JoystickButton6) || Input.GetKey(KeyCode.JoystickButton7) || Input.GetKey(KeyCode.JoystickButton8) || 
            Input.GetKey(KeyCode.JoystickButton9) || Input.GetKey(KeyCode.JoystickButton10) || Input.GetKey(KeyCode.JoystickButton11) || 
            Input.GetKey(KeyCode.JoystickButton12) || Input.GetKey(KeyCode.JoystickButton13) || Input.GetKey(KeyCode.JoystickButton14) || 
            Input.GetKey(KeyCode.JoystickButton15) || Input.GetKey(KeyCode.JoystickButton16) || Input.GetKey(KeyCode.JoystickButton17) || 
            Input.GetKey(KeyCode.JoystickButton18) || Input.GetKey(KeyCode.JoystickButton19) || 
            
            Mathf.Abs(Input.GetAxis("RightStick X")) > 0.1f || Mathf.Abs(Input.GetAxis("RightStick Y")) > 0.1f || Mathf.Abs(Input.GetAxis("LeftStick X")) > 0.1f || 
            Mathf.Abs(Input.GetAxis("LeftStick Y")) > 0.1f || Mathf.Abs(Input.GetAxis("Right Trigger")) > 0.1f || Mathf.Abs(Input.GetAxis("Left Trigger")) > 0.1f ||
            Mathf.Abs(Input.GetAxis("Dpad X")) > 0.1f || Mathf.Abs(Input.GetAxis("Dpad Y")) > 0.1f
            )
        {
            keyboard = false;
        }
        else if (Input.anyKey || Mathf.Abs(Input.GetAxis("Mouse X")) > 0.1f || Mathf.Abs(Input.GetAxis("Mouse Y")) > 0.1f)
        {
            keyboard = true;
        }

        if (
            Input.GetKeyDown(KeyCode.Escape) ||
            Input.GetButtonDown("Start Button")
        )
        {
            if (GameIsPaused && !settingsMenu.activeSelf)
            {
                Resume();
            }
            else if (teaShopMenu.isOpen || equipMenu.isOpen)
            {
                teaShopMenu.CloseMenu();
                equipMenu.CloseMenu();
            }
            else
            {
                if (
                    !gm.gameOver &&
                    !FindObjectOfType<TutorialManager>().tutorialOn
                ) Pause();
            }
        }
        if (keyboard)
        {
            buttonSet = false;
            EventSystem.current.SetSelectedGameObject(null);
        }
        else
        {
            if (buttonSet == false)
            {
                if (pauseMenuUI.activeSelf)
                {
                    SetButton(PauseFirstButton);
                }
                if(settingsMenu.activeSelf)
                {
                    SetButton(settingsFirstButton);
                }
                if (gameover.activeSelf)
                {
                    SetButton(GameOverFirstButton);
                }
                buttonSet = true;
            }
        }
    }

    public void Resume()
    {
        if (!teaShopMenu.isOpen)
        {
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

    public void Pause()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Player.GetComponent<PlayerController>().enabled = false;
        pauseMenuUI.SetActive(true);
        darkenBackground.SetActive(true);
        Time.timeScale = 0.0f;
        GameIsPaused = true;
        SetButton (PauseFirstButton);
        UICanvas.SetActive(false);
    }

    public void SetButton(GameObject buttonToSet)
    {
        //clear selected button
        EventSystem.current.SetSelectedGameObject(null);

        //reassign
        EventSystem.current.SetSelectedGameObject (buttonToSet);
    }

    public void Restart()
    {
        // Time.timeScale = 1f;
        SceneManager.LoadScene("Main Scene");
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void Quit()
    {
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

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen
            .SetResolution(resolution.width,
            resolution.height,
            Screen.fullScreen);
    }

    public void SetBrightness(float brightness)
    {
        _brightnessLevel = brightness;
    }

    public void SetFullScreen(bool isFullscreen)
    {
        _isFullscreen = isFullscreen;
    }

    public void SetQuality(int qualityIndex)
    {
        _qualityLevel = qualityIndex;
    }

    public void SetMasterVolume(float value)
    {
        AudioListener.volume = value;
    }

    public void VolumeApply()
    {
        PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
    }

    public void SetControllerSen(float sensitivity)
    {
        mainControllerSen = Mathf.RoundToInt(sensitivity);
    }

    public void GameplayApply()
    {
        PlayerPrefs.SetFloat("masterSen", mainControllerSen);
    }

    public void ResetButton(string MenuType)
    {
        if (MenuType == "Graphics")
        {
            //reset brightness value
            brightnessSlider.value = defaultBrightness;

            qualityDropdown.value = 1;
            QualitySettings.SetQualityLevel(1);

            fullScreenToggle.isOn = false;
            Screen.fullScreen = false;

            Resolution currentResolution = Screen.currentResolution;
            Screen
                .SetResolution(currentResolution.width,
                currentResolution.height,
                Screen.fullScreen);
            resolutionDropdown.value = 0;
            GraphicsApply();
        }

        if (MenuType == "Audio")
        {
            AudioListener.volume = defaultVolume;
            masterVolumeSlider.value = defaultVolume;
            VolumeApply();
        }

        if (MenuType == "Gameplay")
        {
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
        QualitySettings.SetQualityLevel (_qualityLevel);

        PlayerPrefs.SetInt("masterFullscreen", (_isFullscreen ? 1 : 0));
        Screen.fullScreen = _isFullscreen;
    }
}
