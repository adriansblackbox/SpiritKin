using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_sfx : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] Stepclips;
    [SerializeField]
    private AudioClip[] Slashclips;
    [SerializeField]
    private AudioClip[] Dodgeclips;
    private AudioSource audioSource;
    private bool dodgeSoundPlaying = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    private void Update() {
        if(GetComponent<PlayerCombat>().isDodging &&  !dodgeSoundPlaying){
            Dodge();
        }else if(!GetComponent<PlayerCombat>().isDodging){
            dodgeSoundPlaying = false;
        }
    }

    private void Step()
    {
        if(!GetComponent<PlayerCombat>().isDodging)
        {
            AudioClip clip = GetRandomClip(Stepclips);
            audioSource.pitch = Random.Range(1f, 2f);
            audioSource.PlayOneShot(clip);
        }
    }

    private void Slash()
    {
        AudioClip clip = GetRandomClip(Slashclips);
        audioSource.pitch = Random.Range(1f, 2f);
        audioSource.PlayOneShot(clip);
    }

    private void Dodge()
    {
        dodgeSoundPlaying = true;
        AudioClip clip = GetRandomClip(Dodgeclips);
        audioSource.pitch = Random.Range(1f, 2f);
        audioSource.PlayOneShot(clip);
    }

    private AudioClip GetRandomClip(AudioClip[] cliparray)
    {
        return cliparray[Random.Range(0, cliparray.Length)];
    }
}
