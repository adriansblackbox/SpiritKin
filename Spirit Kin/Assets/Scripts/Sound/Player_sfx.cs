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
    public AudioClip charge, release;
    private bool dodgeSoundPlaying = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    private void Update() {

    }

    private void Step()
    {
        AudioClip clip = GetRandomClip(Stepclips);
        audioSource.pitch = Random.Range(1f, 2f);
        audioSource.PlayOneShot(clip);
    }

    private void Slash()
    {
        AudioClip clip = GetRandomClip(Slashclips);
        //audioSource.pitch = Random.Range(1f, 2f);
        audioSource.PlayOneShot(clip);
    }

    private void Dodge()
    {
        dodgeSoundPlaying = true;
        AudioClip clip = GetRandomClip(Dodgeclips);
        //audioSource.pitch = Random.Range(1f, 1.5f);
        audioSource.PlayOneShot(clip);
    }

    private void Charge()
    {
        audioSource.PlayOneShot(charge);
    }

    private void Release()
    {
        audioSource.PlayOneShot(release);
    }
    private AudioClip GetRandomClip(AudioClip[] cliparray)
    {
        return cliparray[Random.Range(0, cliparray.Length)];
    }
}
