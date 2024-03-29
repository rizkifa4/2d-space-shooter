using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int _health = 3;
    [SerializeField] private float _speed;
    [SerializeField] private float _speedMultiplier = 2;
    [SerializeField] private Vector3 _initialPosition;
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private GameObject _tripleShotPrefab;
    [SerializeField] private Transform _bulletSpawnPoint;
    [SerializeField] private float _fireRate;
    [SerializeField] private SpawnManager _spawnManager;
    private float _nextFire = 0f;
    private bool _isTripleShotEnabled;
    private bool _isShieldEnabled;
    [SerializeField] private GameObject _playerShield;
    [SerializeField] private GameObject _fireRightEngine;
    [SerializeField] private GameObject _fireLeftEngine;
    [SerializeField] private GameObject _playerThruster;
    [SerializeField] private int _score;
    [SerializeField] private int _highScore;
    [SerializeField] private Animator _playerAnim;
    private bool _isTurnRight;
    [SerializeField] private SoundManager _soundManager;

    void Start()
    {
        _bulletSpawnPoint = transform.GetChild(0);
        transform.position = _initialPosition;

        _spawnManager.GetComponent<SpawnManager>();

        _playerShield.SetActive(false);

        _highScore = PlayerPrefs.GetInt("HighScore", _highScore);

        UIManager.Instance.UpdateScore(_score, _highScore);
        UIManager.Instance.UpdateLives(_health);

        _fireRightEngine.SetActive(false);
        _fireLeftEngine.SetActive(false);

        _playerAnim = GetComponent<Animator>();

        _soundManager.GetComponent<SoundManager>();
    }

    void Update()
    {
        Movement();

        Shoot();
    }

    private void Movement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        float xRange = 11;

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);
        transform.Translate(direction * _speed * Time.deltaTime);

        if (horizontalInput > 0)
        {
            _playerAnim.SetBool("TurnRight", true);
            _playerAnim.SetBool("TurnLeft", false);

            _isTurnRight = true;
        }
        else if (horizontalInput < 0)
        {
            _playerAnim.SetBool("TurnLeft", true);
            _playerAnim.SetBool("TurnRight", false);

            _isTurnRight = false;
        }

        if (horizontalInput == 0)
        {
            if (_isTurnRight)
            {
                _playerAnim.SetBool("TurnRight", false);
            }
            else
            {
                _playerAnim.SetBool("TurnLeft", false);
            }
        }


        if (transform.position.x < -xRange)
        {
            transform.position = new Vector3(xRange, transform.position.y, transform.position.z);
        }
        if (transform.position.x > xRange)
        {
            transform.position = new Vector3(-xRange, transform.position.y, transform.position.z);
        }

        if (transform.position.y >= 0)
        {
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        }
        if (transform.position.y <= -4f)
        {
            transform.position = new Vector3(transform.position.x, -4f, transform.position.z);
        }
    }

    private void Shoot()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _nextFire)
        {
            _nextFire = Time.time + _fireRate;
            FireBullet();

            _soundManager.PlaySoundEffect(_soundManager.playerLaserSound);
        }
    }

    private void FireBullet()
    {
        if (!_isTripleShotEnabled)
        {
            Instantiate(_laserPrefab, _bulletSpawnPoint.position, Quaternion.identity);
        }
        else
        {
            Instantiate(_tripleShotPrefab, _bulletSpawnPoint.position, Quaternion.identity);
        }
    }

    public void Damage(int damage)
    {
        if (!_isShieldEnabled)
        {
            _health -= damage;
            UIManager.Instance.UpdateLives(_health);

            if (_health <= 2 && _health > 1)
            {
                _fireRightEngine.SetActive(true);
            }
            else if (_health <= 1 && _health > 0)
            {
                _fireLeftEngine.SetActive(true);
            }
            else
            {
                _fireRightEngine.SetActive(false);
                _fireLeftEngine.SetActive(false);
                _playerThruster.SetActive(false);
                _speed = 0;

                _spawnManager.OnPlayerDeath();

                _playerAnim.SetTrigger("Explode");
                _soundManager.PlaySoundEffect(_soundManager.explosionSound);

                float destroyDelay = 2.5f;
                Destroy(gameObject, destroyDelay);
            }
        }
    }

    public void TripleShotActive()
    {
        _isTripleShotEnabled = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    private IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5f);
        _isTripleShotEnabled = false;
    }

    public void SpeedPowerupActive()
    {
        _speed *= _speedMultiplier;
        StartCoroutine(SpeedPowerDownRoutine());
    }

    private IEnumerator SpeedPowerDownRoutine()
    {
        yield return new WaitForSeconds(5f);
        _speed /= _speedMultiplier;
    }

    public void ShieldPowerupActive()
    {
        _isShieldEnabled = true;
        _playerShield.SetActive(true);
        StartCoroutine(ShieldPowerDownRoutine());
    }

    private IEnumerator ShieldPowerDownRoutine()
    {
        yield return new WaitForSeconds(5f);
        _isShieldEnabled = false;
        _playerShield.SetActive(false);
    }

    public void AddScore(int points)
    {
        _score += points;

        if (_score > _highScore)
        {
            _highScore = _score;
            PlayerPrefs.SetInt("HighScore", _highScore);
        }

        UIManager.Instance.UpdateScore(_score, _highScore);
    }
}
