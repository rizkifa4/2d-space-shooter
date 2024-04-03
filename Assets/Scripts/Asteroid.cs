using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField] private float _rotateSpeed;
    [SerializeField] private GameObject _explosionPrefab;
    private SpawnManager _spawnManager;
    private SoundManager _soundManager;

    private void Start()
    {
        _spawnManager = GameObject.FindWithTag("SpawnManager").GetComponent<SpawnManager>();

        _soundManager = GameObject.FindWithTag("SoundManager").GetComponent<SoundManager>();

        // if (_spawnManager != null)
        // {
        //     Debug.Log("Spawn Manager is not null");
        // }

        // if (_soundManager != null)
        // {
        //     Debug.Log("Sound Manager is not null");
        // }
    }

    void Update()
    {
        transform.Rotate(Vector3.forward * _rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "PlayerLaser")
        {
            Destroy(other.gameObject);

            _soundManager.PlaySoundEffect(_soundManager.explosionSound);

            GameObject explosion = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            Destroy(explosion, 2.62f);
            Destroy(this.gameObject);

            _spawnManager.StartSpawning();
        }
    }
}
