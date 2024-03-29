using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private AudioSource _audioSource;

    public AudioClip playerLaserSound;
    public AudioClip explosionSound;
    public AudioClip powerupSound;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlaySoundEffect(AudioClip clip) => _audioSource.PlayOneShot(clip);
}
