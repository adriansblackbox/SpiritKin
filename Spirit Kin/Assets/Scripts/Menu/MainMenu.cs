using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    public AudioSource PlaybuttonSFX;
    public AudioSource OptionbuttonSFX;
    private bool check = false;
    public void PlayGame (){
        PlaybuttonSFX.Play();
        check = true;
    }

    public void QuitGame (){
        Application.Quit();
    }

    public void Option()
    {
        OptionbuttonSFX.Play();
    }

    public void Update()
    {
        if (!PlaybuttonSFX.isPlaying && check)
        {
            check = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
