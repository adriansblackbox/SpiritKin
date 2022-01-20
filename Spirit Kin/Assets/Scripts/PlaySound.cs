using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    AudioSource AudioSourceStatic;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Playsound(AudioSource source, AudioClip clip, float pitch)
    {
        AudioSourceStatic.PlayOneShot(clip);
    }
}


