using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("High Score Text")]
    [SerializeField]
    TMP_Text easyHighScore;

    [SerializeField]
    TMP_Text hardHighScore;

    [Header("Volume Settings")]
    [SerializeField]
    private TMP_Text masterVolumeValue = null;

    [SerializeField]
    private Slider masterVolumeSlider = null;

    [SerializeField]
    private float defaultVolume = 0.5f;

    [Header("Gameplay Settings")]
    [SerializeField]
    private TMP_Text controllerSenTextValue = null;

    [SerializeField]
    private Slider controllerSenSlider = null;

    [SerializeField]
    private int defaultSen = 4;

    public int mainControllerSen = 4;

    [Header("Toggle Settings")]
    [SerializeField]
    private Toggle invertYToggle = null;

    [Header("Graphics Settings")]
    [SerializeField]
    private Slider brightnessSlider = null;

    [SerializeField]
    private TMP_Text brightnessTextValue = null;

    [SerializeField]
    private int defaultBrightness = 4;

    [Space(10)]
    [SerializeField]
    private TMP_Dropdown qualityDropdown;

    [SerializeField]
    private Toggle fullScreenToggle;

    private int _qualityLevel;

    private bool _isFullscreen;

    private float _brightnessLevel;

    [Header("Confirmation")]
    [SerializeField]
    private GameObject confirmationPrompt = null;

    [Header("Resolution DropDown")]
    public TMP_Dropdown resolutionDropdown;

    private Resolution[] resolutions;

    [Header("First Buttons")]
    [SerializeField]
    private GameObject playButton;

    public bool keyboard = false;

    public bool buttonSet = false;

    [SerializeField]
    private GameObject playMode;

    [SerializeField]
    private GameObject playModeFirstButton;

    [SerializeField]
    private GameObject options;

    [SerializeField]
    private GameObject optionsFirstButton;

    [SerializeField]
    private GameObject settings;

    [SerializeField]
    private GameObject settingsFirstButton;

    [SerializeField]
    private GameObject graphics;

    [SerializeField]
    private GameObject graphicsFirstButton;

    [SerializeField]
    private GameObject credit;

    [SerializeField]
    private GameObject creditFirstButton;

    [SerializeField]
    private GameObject exit;

    [SerializeField]
    private GameObject exitFirstButton;

    private void Start()
    {
        if (PlayerPrefs.HasKey("highScoreEasy"))
            easyHighScore.text =
                "High Score\n" + PlayerPrefs.GetInt("highScoreEasy").ToString();
        if (PlayerPrefs.HasKey("highScoreHard"))
            hardHighScore.text =
                "High Score\n" + PlayerPrefs.GetInt("highScoreHard").ToString();

        // SetFirstButton (playButton);
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option =
                resolutions[i].width + " x " + resolutions[i].height;

            // if (!options.Contains(option))
            options.Add (option);

            if (
                resolutions[i].width == Screen.width &&
                resolutions[i].height == Screen.height
            )
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions (options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        SetFullScreen(true);
        GraphicsApply();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen
            .SetResolution(resolution.width,
            resolution.height,
            Screen.fullScreen);
    }

    public void SetFirstButton(GameObject button)
    {
        //clear selected button
        EventSystem.current.SetSelectedGameObject(null);

        //reassign
        EventSystem.current.SetSelectedGameObject (button);
    }

    public AudioMixer mixer;

    public AudioClip PlaybuttonSFX;

    public AudioClip OptionbuttonSFX;

    public AudioSource MenuSounds;

    private bool check = false;

    private bool faded = false;

    public void PlayGame()
    {
        MenuSounds.PlayOneShot (PlaybuttonSFX);
        check = true;
        faded = true;

        SceneManager.LoadScene("Main Scene");
        Time.timeScale = 1f;
    }

    public void QuitGame()
    {
        if (faded) return;
        Application.Quit();
    }

    public void Option()
    {
        if (faded) return;
        MenuSounds.PlayOneShot (OptionbuttonSFX);
    }

    public void Back()
    {
        if (faded) return;
        MenuSounds.PlayOneShot (OptionbuttonSFX);
    }

    public void Back2()
    {
        if (faded) return;
        MenuSounds.PlayOneShot (OptionbuttonSFX);
    }

    public void Credit()
    {
        if (faded) return;
        MenuSounds.PlayOneShot (OptionbuttonSFX);
    }

    public void Update()
    {
        //controller handler
        if (
            Input.GetKey(KeyCode.JoystickButton0) ||
            Input.GetKey(KeyCode.JoystickButton1) ||
            Input.GetKey(KeyCode.JoystickButton2) ||
            Input.GetKey(KeyCode.JoystickButton3) ||
            Input.GetKey(KeyCode.JoystickButton4) ||
            Input.GetKey(KeyCode.JoystickButton5) ||
            Input.GetKey(KeyCode.JoystickButton6) ||
            Input.GetKey(KeyCode.JoystickButton7) ||
            Input.GetKey(KeyCode.JoystickButton8) ||
            Input.GetKey(KeyCode.JoystickButton9) ||
            Input.GetKey(KeyCode.JoystickButton10) ||
            Input.GetKey(KeyCode.JoystickButton11) ||
            Input.GetKey(KeyCode.JoystickButton12) ||
            Input.GetKey(KeyCode.JoystickButton13) ||
            Input.GetKey(KeyCode.JoystickButton14) ||
            Input.GetKey(KeyCode.JoystickButton15) ||
            Input.GetKey(KeyCode.JoystickButton16) ||
            Input.GetKey(KeyCode.JoystickButton17) ||
            Input.GetKey(KeyCode.JoystickButton18) ||
            Input.GetKey(KeyCode.JoystickButton19) ||
            Mathf.Abs(Input.GetAxis("RightStick X")) > 0.1f ||
            Mathf.Abs(Input.GetAxis("RightStick Y")) > 0.1f ||
            Mathf.Abs(Input.GetAxis("LeftStick X")) > 0.1f ||
            Mathf.Abs(Input.GetAxis("LeftStick Y")) > 0.1f ||
            Mathf.Abs(Input.GetAxis("Right Trigger")) > 0.1f ||
            Mathf.Abs(Input.GetAxis("Left Trigger")) > 0.1f ||
            Mathf.Abs(Input.GetAxis("Dpad X")) > 0.1f ||
            Mathf.Abs(Input.GetAxis("Dpad Y")) > 0.1f
        )
        {
            keyboard = false;
        }
        else if (
            Input.anyKey ||
            Mathf.Abs(Input.GetAxis("Mouse X")) > 0.1f ||
            Mathf.Abs(Input.GetAxis("Mouse Y")) > 0.1f
        )
        {
            keyboard = true;
        }
        if (keyboard)
        {
            //clear selected button
            EventSystem.current.SetSelectedGameObject(null);
            buttonSet = false;
        }
        else
        {
            if (!buttonSet)
            {
                SetFirstButton (playButton);
                if (playMode.activeSelf)
                {
                    SetFirstButton(playModeFirstButton);
                }
                if (options.activeSelf)
                {
                    SetFirstButton(optionsFirstButton);
                }
                if (graphics.activeSelf)
                {
                    SetFirstButton(graphicsFirstButton);
                }
                if (settings.activeSelf)
                {
                    SetFirstButton(settingsFirstButton);
                }
                if (credit.activeSelf)
                {
                    SetFirstButton(creditFirstButton);
                }
                if (exit.activeSelf)
                {
                    SetFirstButton(exitFirstButton);
                }
                buttonSet = true;
            }
        }
        if (!MenuSounds.isPlaying && check)
        {
            check = false;
            SceneManager
                .LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    public IEnumerator playDelay()
    {
        yield return new WaitForSeconds(1.0f);
    }

    public void SetMasterVolume(float value)
    {
        // mixer.SetFloat("MasterVolume", value);
        AudioListener.volume = value;
        masterVolumeValue.text = value.ToString("0.00");
    }

    public void VolumeApply()
    {
        PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
        StartCoroutine(ConfirmationBox());
    }

    public void SetControllerSen(float sensitivity)
    {
        mainControllerSen = Mathf.RoundToInt(sensitivity);
        controllerSenTextValue.text = sensitivity.ToString("0");
    }

    public void GameplayApply()
    {
        if (invertYToggle.isOn)
        {
            PlayerPrefs.SetInt("masterInvertY", 1);
            //invert Y
        }
        else
        {
            PlayerPrefs.SetInt("masterInvertY", 0);
            //Not invert
        }

        PlayerPrefs.SetFloat("masterSen", mainControllerSen);
        StartCoroutine(ConfirmationBox());
    }

    public void SetBrightness(float brightness)
    {
        _brightnessLevel = brightness;
        brightnessTextValue.text = brightness.ToString("0.0");
    }

    public void SetFullScreen(bool isFullscreen)
    {
        _isFullscreen = isFullscreen;
    }

    public void SetQuality(int qualityIndex)
    {
        _qualityLevel = qualityIndex;
    }

    public void GraphicsApply()
    {
        PlayerPrefs.SetFloat("masterBrightness", _brightnessLevel);

        //change your brightness with your post processing or whatever it is
        PlayerPrefs.SetInt("masterQuality", _qualityLevel);
        QualitySettings.SetQualityLevel (_qualityLevel);

        PlayerPrefs.SetInt("masterFullscreen", (_isFullscreen ? 1 : 0));
        Screen.fullScreen = _isFullscreen;

        StartCoroutine(ConfirmationBox());
    }

    public void ResetButton(string MenuType)
    {
        if (MenuType == "Graphics")
        {
            //reset brightness value
            brightnessSlider.value = defaultBrightness;
            brightnessTextValue.text = defaultBrightness.ToString("0,0");

            qualityDropdown.value = 1;
            QualitySettings.SetQualityLevel(1);

            fullScreenToggle.isOn = true;
            Screen.fullScreen = true;

            Resolution currentResolution = Screen.currentResolution;
            Screen
                .SetResolution(currentResolution.width,
                currentResolution.height,
                Screen.fullScreen);
            resolutionDropdown.value = resolutions.Length;
            GraphicsApply();
        }

        if (MenuType == "Audio")
        {
            AudioListener.volume = defaultVolume;
            masterVolumeSlider.value = defaultVolume;
            masterVolumeValue.text = defaultVolume.ToString("0.0");
            VolumeApply();
        }

        if (MenuType == "Gameplay")
        {
            controllerSenTextValue.text = defaultSen.ToString("0");
            controllerSenSlider.value = defaultSen;
            mainControllerSen = defaultSen;
            invertYToggle.isOn = false;
            GameplayApply();
        }
    }

    public void OpenLink()
    {
        Application.OpenURL("https://yliu637.wixsite.com/spiritkin");
    }

    public IEnumerator ConfirmationBox()
    {
        confirmationPrompt.SetActive(true);
        yield return new WaitForSeconds(2);
        confirmationPrompt.SetActive(false);
    }

    public void SetSFXVolume(float value)
    {
        mixer.SetFloat("SFXVolume", value);
    }

    public void SetMusicVolume(float value)
    {
        mixer.SetFloat("MusicVolume", value);
    }
}
