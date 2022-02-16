using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    public AudioClip PlaybuttonSFX;
    public AudioClip OptionbuttonSFX;
    public AudioSource MenuSounds;
    private bool check = false;
    public void PlayGame (){
        MenuSounds.PlayOneShot(PlaybuttonSFX);
        check = true;
    }

    public void QuitGame (){
        Application.Quit();
    }

    public void Option()
    {
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
}
