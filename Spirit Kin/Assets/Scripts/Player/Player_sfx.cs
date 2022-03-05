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

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
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
        audioSource.pitch = Random.Range(1f, 2f);
        audioSource.PlayOneShot(clip);
    }

    private void Dodge()
    {
        AudioClip clip = GetRandomClip(Dodgeclips);
        audioSource.pitch = Random.Range(1f, 2f);
        audioSource.PlayOneShot(clip);
    }

    private AudioClip GetRandomClip(AudioClip[] cliparray)
    {
        return cliparray[Random.Range(0, cliparray.Length)];
    }
}
