using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private AudioSource _audioSource;

    [SerializeField] private AudioClip _playerLaserSound;
    [SerializeField] private AudioClip _explosionSound;
    [SerializeField] private AudioClip _positivePowerUpSound;
    [SerializeField] private AudioClip _negativePowerUpSound;

    public AudioClip PlayerLaserSound => _playerLaserSound;
    public AudioClip ExplosionSound => _explosionSound;
    public AudioClip PositivePowerUpSound => _positivePowerUpSound;
    public AudioClip NegativePowerUpSound => _negativePowerUpSound;


    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlaySoundEffect(AudioClip clip) => _audioSource.PlayOneShot(clip);
}
