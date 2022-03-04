using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_sfx : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] Stepclips;
    public AudioClip slash1, slash2, slash3;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Step()
    {
        AudioClip clip = GetRandomClip();
        audioSource.pitch = Random.Range(1f, 2f);
        audioSource.PlayOneShot(clip);
    }

    private void Slash()
    {
        audioSource.PlayOneShot(slash1);
        /*switch (order)
        {
            case 1:
                audioSource.PlayOneShot(slash1);
                break;
            case 2:
                audioSource.PlayOneShot(slash2);
                break;
            case 3:
                audioSource.PlayOneShot(slash3);
                break;
        }*/
    }

    private AudioClip GetRandomClip()
    {
        return Stepclips[Random.Range(0, Stepclips.Length)];
    }
}
