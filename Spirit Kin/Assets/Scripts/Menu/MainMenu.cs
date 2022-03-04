using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class MainMenu : MonoBehaviour
{
    public AudioMixer mixer;
    public AudioClip PlaybuttonSFX;
    public AudioClip OptionbuttonSFX;
    public AudioSource MenuSounds;

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
    }

    public void Credit()
    {
        if (faded) return;
        MenuSounds.PlayOneShot(OptionbuttonSFX);
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
        yield return new WaitForSeconds(3.0f);
    }

    public void SetMasterVolume (float value){
        mixer.SetFloat("MasterVolume", value);
    }

    public void SetSFXVolume(float value)
    {
        mixer.SetFloat("SFXVolume", value);
    }
}
