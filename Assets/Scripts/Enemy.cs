using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _speed = 4f;
    [SerializeField] private int _damageAmount;
    [SerializeField] private int _scoreValue;
    [SerializeField] private Transform _laserSpawnPoint;
    [SerializeField] private GameObject _laserPrefab;
    private Player _player;
    private Animator _enemyAnim;
    private bool _isDead;
    private SoundManager _soundManager;
    private float _fireRate = 3.0f;
    private float _canFire = -1;

    void Start()
    {
        _player = GameObject.FindWithTag("Player").GetComponent<Player>();

        _enemyAnim = GetComponent<Animator>();

        _soundManager = GameObject.FindWithTag("SoundManager").GetComponent<SoundManager>();
    }

    void Update()
    {
        Movement();

        EnemyFire();
    }

    private void Movement()
    {
        if (!_isDead)
        {
            transform.Translate(Vector3.down * _speed * Time.deltaTime);

            float yBound = -5.5f;

            if (transform.position.y < yBound)
            {
                float xMin = -9f;
                float xMax = 9f;
                float randomX = Random.Range(xMin, xMax);

                float yPos = 7;
                float xPos = randomX;

                transform.position = new Vector3(xPos, yPos, 0);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && !_isDead)
        {
            Player player = other.GetComponent<Player>();

            if (player != null)
            {
                DestroyEnemy();

                player.Damage(_damageAmount);
            }

            _enemyAnim.SetTrigger("Destroy");
        }

        if (other.tag == "PlayerLaser" && !_isDead)
        {
            if (_player != null)
            {
                DestroyEnemy();

                _player.AddScore(_scoreValue);
            }

            _enemyAnim.SetTrigger("Destroy");

            Destroy(other.gameObject);
        }
    }

    private void DestroyEnemy()
    {
        _isDead = true;

        float destroyDelay = 2.5f;
        Destroy(this.gameObject, destroyDelay);

        _soundManager.PlaySoundEffect(_soundManager.explosionSound);
    }

    private void EnemyFire()
    {
        if (Time.time > _canFire && !_isDead)
        {
            _fireRate = Random.Range(3f, 7f);
            _canFire = Time.time + _fireRate;

            Instantiate(_laserPrefab, _laserSpawnPoint.position, Quaternion.identity);

            _soundManager.PlaySoundEffect(_soundManager.explosionSound);
        }
    }
}
