using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Header("Volume Settings")]
    [SerializeField] private TMP_Text masterVolumeValue = null;
    [SerializeField] private Slider masterVolumeSlider = null;
    [SerializeField] private float defaultVolume = 0.5f;

    [Header("Gameplay Settings")]
    [SerializeField] private TMP_Text controllerSenTextValue = null;
    [SerializeField] private Slider controllerSenSlider = null;
    [SerializeField] private int defaultSen = 4;
    public int mainControllerSen = 4;

    [Header("Toggle Settings")]
    [SerializeField] private Toggle invertYToggle = null;

    [Header("Graphics Settings")]
    [SerializeField] private Slider brightnessSlider = null;
    [SerializeField] private TMP_Text brightnessTextValue = null;
    [SerializeField] private int defaultBrightness = 4;

    [Space(10)]
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private Toggle fullScreenToggle;

    private int _qualityLevel;
    private bool _isFullscreen;
    private float _brightnessLevel;

    [Header("Confirmation")]
    [SerializeField] private GameObject confirmationPrompt = null;

    [Header("Resolution DropDown")]
    public TMP_Dropdown resolutionDropdown;
    private Resolution[] resolutions;

    private void Start(){
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        
        for(int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if(resolutions[i].width == Screen.width && resolutions[i].height == Screen.height){
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution(int resolutionIndex){
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public AudioMixer mixer;
    public AudioClip PlaybuttonSFX;
    public AudioClip OptionbuttonSFX;
    public AudioSource MenuSounds;

    public GameObject OptionFirstButton, OptionCloseButton, CreditFirstButton, CreditCloseButton;

    private bool check = false;
    private bool faded = false;
    public void PlayGame (){
        MenuSounds.PlayOneShot(PlaybuttonSFX);
        check = true;
        faded = true;

        SceneManager.LoadScene("Main Scene");
        Time.timeScale = 1f;
    }

    public void QuitGame (){
        if (faded) return;
        Application.Quit();
    }

    public void Option()
    {
        if (faded) return;
        MenuSounds.PlayOneShot(OptionbuttonSFX);
        //clear selected button
        EventSystem.current.SetSelectedGameObject(null);
        //reassign
        EventSystem.current.SetSelectedGameObject(OptionFirstButton);
    }

    public void Back(){
        if (faded) return;
        MenuSounds.PlayOneShot(OptionbuttonSFX);
        //clear selected button
        EventSystem.current.SetSelectedGameObject(null);
        //reassign
        EventSystem.current.SetSelectedGameObject(OptionCloseButton);
    } 
    public void Back2(){
        if (faded) return;
        MenuSounds.PlayOneShot(OptionbuttonSFX);
        //clear selected button
        EventSystem.current.SetSelectedGameObject(null);
        //reassign
        EventSystem.current.SetSelectedGameObject(CreditCloseButton);
    } 

    public void Credit()
    {
        if (faded) return;
        MenuSounds.PlayOneShot(OptionbuttonSFX);
        //clear selected button
        EventSystem.current.SetSelectedGameObject(null);
        //reassign
        EventSystem.current.SetSelectedGameObject(CreditFirstButton);
    }

    public void Update()
    {
        if (!MenuSounds.isPlaying && check)
        {
            check = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    public IEnumerator playDelay() {
        yield return new WaitForSeconds(1.0f);
    }

    public void SetMasterVolume (float value){
        // mixer.SetFloat("MasterVolume", value);
        AudioListener.volume = value;
        masterVolumeValue.text = value.ToString("0.0");
    }

    public void VolumeApply(){
        PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
        StartCoroutine(ConfirmationBox());
    }

    public void SetControllerSen(float sensitivity){
        mainControllerSen = Mathf.RoundToInt(sensitivity);
        controllerSenTextValue.text = sensitivity.ToString("0");
    }

    public void GameplayApply(){
        if(invertYToggle.isOn){
            PlayerPrefs.SetInt("masterInvertY", 1);
            //invert Y
        }else{
            PlayerPrefs.SetInt("masterInvertY", 0);
            //Not invert
        }

        PlayerPrefs.SetFloat("masterSen", mainControllerSen);
        StartCoroutine(ConfirmationBox());
    }

    public void SetBrightness(float brightness){
        _brightnessLevel = brightness;
        brightnessTextValue.text = brightness.ToString("0.0");
    }

    public void SetFullScreen(bool isFullscreen){
        _isFullscreen = isFullscreen;
    }

    public void SetQuality(int qualityIndex){
        _qualityLevel = qualityIndex;
    }

    public void GraphicsApply(){
        PlayerPrefs.SetFloat("masterBrightness", _brightnessLevel);
        //change your brightness with your post processing or whatever it is

        PlayerPrefs.SetInt("masterQuality", _qualityLevel);
        QualitySettings.SetQualityLevel(_qualityLevel);

        PlayerPrefs.SetInt("masterFullscreen", (_isFullscreen ? 1:0));
        Screen.fullScreen = _isFullscreen;

        StartCoroutine(ConfirmationBox());
    }

    public void ResetButton(string MenuType){

        if(MenuType == "Graphics"){
            //reset brightness value
            brightnessSlider.value = defaultBrightness;
            brightnessTextValue.text = defaultBrightness.ToString("0,0");

            qualityDropdown.value = 1;
            QualitySettings.SetQualityLevel(1);

            fullScreenToggle.isOn = false;
            Screen.fullScreen = false;

            Resolution currentResolution = Screen.currentResolution;
            Screen.SetResolution(currentResolution.width, currentResolution.height, Screen.fullScreen);
            resolutionDropdown.value = resolutions.Length;
            GraphicsApply();
        }

        if(MenuType == "Audio"){
            AudioListener.volume = defaultVolume;
            masterVolumeSlider.value = defaultVolume;
            masterVolumeValue.text = defaultVolume.ToString("0.0");
            VolumeApply();
        }

        if(MenuType == "Gameplay"){
            controllerSenTextValue.text = defaultSen.ToString("0");
            controllerSenSlider.value = defaultSen;
            mainControllerSen = defaultSen;
            invertYToggle.isOn = false;
            GameplayApply();
        }
    }

    public IEnumerator ConfirmationBox(){
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
