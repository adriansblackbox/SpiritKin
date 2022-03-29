using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DeathManager : MonoBehaviour
{
    public AudioClip DeathSFX;
    public AudioSource Sounds;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void openScene()
    {
        Sounds.PlayOneShot(DeathSFX,1);
        gameObject.SetActive(true);
    }
    public void closeScene()
    {
        gameObject.SetActive(false);
    }
}
