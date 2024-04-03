using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int _health = 3;
    [SerializeField] private float _speed;
    [SerializeField] private Vector3 _initialPosition;
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private GameObject _tripleShotPrefab;
    [SerializeField] private Transform _bulletSpawnPoint;
    [SerializeField] private float _fireRate;
    [SerializeField] private SpawnManager _spawnManager;
    private float _nextFire = 0f;
    private float _tripleshotDuration = 5f;
    private float _shieldDuration = 5f;
    private float _initialSpeedBoostDuration = 5f;
    private float _speedBoostDuration;
    private float _initialSpeedDecreaseDuration = 5f;
    private float _speedDecreaseDuration;
    private float _originalSpeed = 5f;
    private float _speedBoost = 10f;
    private float _speedDecrease = 2.5f;

    private bool _isTripleShotEnabled;
    private bool _isShieldEnabled;
    private bool _isSpeedBoostEnabled;
    private bool _isSpeedDecreaseEnabled;
    [SerializeField] private GameObject _playerShield;
    [SerializeField] private GameObject _rightEngineDamage;
    [SerializeField] private GameObject _leftEngineDamage;
    [SerializeField] private GameObject _playerThruster;
    private int _score;
    private int _highScore;
    [SerializeField] private Animator _playerAnim;
    [SerializeField] private int _laserAmmo;
    [SerializeField] private int _maxAmmo;
    private bool _isTurnRight;
    private bool _isDead;
    [SerializeField] private SoundManager _soundManager;

    public bool IsTripleShotEnabled { get { return _isTripleShotEnabled; } }
    public bool IsShieldEnabled { get { return _isShieldEnabled; } }
    public bool IsSpeedBoostEnabled { get { return _isSpeedBoostEnabled; } }
    public bool IsSpeedDecreaseEnabled { get { return _isSpeedDecreaseEnabled; } }
    public int Health { get { return _health; } }
    public int CurrentAmmo { get { return _laserAmmo; } }
    public int MaxAmmo { get { return _maxAmmo; } }

    public bool IsSpeedBoostActive()
    {
        return _isSpeedBoostEnabled;
    }

    public bool IsShieldActive()
    {
        return _isShieldEnabled;
    }

    public bool IsTripleShotActive()
    {
        return _isTripleShotEnabled;
    }

    void Start()
    {
        _bulletSpawnPoint = transform.GetChild(0);
        transform.position = _initialPosition;

        _spawnManager.GetComponent<SpawnManager>();

        _playerShield.SetActive(false);

        _speedBoostDuration = _initialSpeedBoostDuration;
        _speedDecreaseDuration = _initialSpeedDecreaseDuration;

        _highScore = PlayerPrefs.GetInt("HighScore", _highScore);

        UIManager.Instance.UpdateScore(_score, _highScore);
        UIManager.Instance.UpdateLives(_health);
        UIManager.Instance.UpdateAmmo(_laserAmmo, _maxAmmo);

        _rightEngineDamage.SetActive(false);
        _leftEngineDamage.SetActive(false);

        _playerAnim = GetComponent<Animator>();

        _soundManager.GetComponent<SoundManager>();
    }

    void Update()
    {
        Movement();

        LaserShoot();

        SetBoostAndDecreaseSpeedTimer();
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

    private void LaserShoot()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FireLaser();
        }
    }

    private void FireLaser()
    {
        if (Time.time > _nextFire && _laserAmmo > 0)
        {
            GameObject bullet = !_isTripleShotEnabled ? _laserPrefab : _tripleShotPrefab;
            Instantiate(bullet, _bulletSpawnPoint.position, Quaternion.identity);

            _laserAmmo--;
            UIManager.Instance.UpdateAmmo(_laserAmmo, _maxAmmo);

            _soundManager.PlaySoundEffect(_soundManager.playerLaserSound);
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
                int random = Random.Range(0, 2);
                if (random == 0) _rightEngineDamage.SetActive(true); else _leftEngineDamage.SetActive(true);
            }
            else if (_health <= 1 && _health > 0)
            {
                if (_rightEngineDamage.activeSelf)
                {
                    _leftEngineDamage.SetActive(true);
                }
                else
                {
                    if (_rightEngineDamage.activeSelf) _leftEngineDamage.SetActive(true); else _rightEngineDamage.SetActive(true);
                }
            }
            else
            {
                _rightEngineDamage.SetActive(false);
                _leftEngineDamage.SetActive(false);
                _playerThruster.SetActive(false);

                _spawnManager.OnPlayerDeath();

                _playerAnim.SetTrigger("Explode");
                _soundManager.PlaySoundEffect(_soundManager.explosionSound);
                Destroy(GetComponent<Collider2D>());

                _isDead = true;
                float destroyDelay = 2.5f;
                Destroy(gameObject, destroyDelay);
            }

            _soundManager.PlaySoundEffect(_soundManager.explosionSound);
        }
    }

    public void TripleShotActive()
    {
        _isTripleShotEnabled = true;

        _soundManager.PlaySoundEffect(_soundManager.positivePowerUpSound);

        StartCoroutine(TripleShotPowerDownRoutine(_tripleshotDuration));
    }

    private IEnumerator TripleShotPowerDownRoutine(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        _isTripleShotEnabled = false;
    }

    public void SpeedBoostPowerupActive()
    {
        _isSpeedDecreaseEnabled = false;

        _speedBoostDuration = _initialSpeedBoostDuration;
        _speed = _speedBoost;

        _isSpeedBoostEnabled = true;

        _soundManager.PlaySoundEffect(_soundManager.positivePowerUpSound);
    }

    public void SpeedDecreasePowerupActive()
    {
        _isSpeedBoostEnabled = false;

        _speedDecreaseDuration = _initialSpeedDecreaseDuration;
        _speed = _speedDecrease;

        _isSpeedDecreaseEnabled = true;

        _soundManager.PlaySoundEffect(_soundManager.negativePowerUpSound);
    }

    public void ShieldPowerupActive()
    {
        _isShieldEnabled = true;
        _playerShield.SetActive(true);

        _soundManager.PlaySoundEffect(_soundManager.positivePowerUpSound);

        StartCoroutine(ShieldPowerDownRoutine(_shieldDuration));
    }

    private IEnumerator ShieldPowerDownRoutine(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
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

    public void RestoreHealth()
    {
        _health++;
        UIManager.Instance.UpdateLives(_health);

        if (_health == 3)
        {
            if (_rightEngineDamage.activeSelf) _rightEngineDamage.SetActive(false); else _leftEngineDamage.SetActive(false);
        }
        else if (_health == 2)
        {
            int random = Random.Range(0, 2);
            if (random == 0) _rightEngineDamage.SetActive(false); else _leftEngineDamage.SetActive(false);
        }

        _soundManager.PlaySoundEffect(_soundManager.positivePowerUpSound);
    }

    public void RestoreAmmo(int ammo)
    {
        _laserAmmo += ammo;

        if (_laserAmmo > _maxAmmo)
        {
            _laserAmmo = _maxAmmo;
        }

        UIManager.Instance.UpdateAmmo(_laserAmmo, _maxAmmo);

        _soundManager.PlaySoundEffect(_soundManager.positivePowerUpSound);
    }

    private void SetBoostAndDecreaseSpeedTimer()
    {
        if (!_isDead)
        {
            if (_isSpeedBoostEnabled)
            {
                _speedBoostDuration -= Time.deltaTime;

                if (_speedBoostDuration <= 0f)
                {
                    _isSpeedBoostEnabled = false;
                    _speed = _originalSpeed;
                }

                UIManager.Instance.UpdateSpeedFillAndTimer(_speed, _speedBoostDuration);
            }
            else if (_isSpeedDecreaseEnabled)
            {
                _speedDecreaseDuration -= Time.deltaTime;

                if (_speedDecreaseDuration <= 0f)
                {
                    _isSpeedDecreaseEnabled = false;
                    _speed = _originalSpeed;
                }

                UIManager.Instance.UpdateSpeedFillAndTimer(_speed, _speedDecreaseDuration);
            }
            else
            {
                _speed = _originalSpeed;
                UIManager.Instance.UpdateSpeedFillAndTimer(_speed, 0f);
            }
        }
        else
        {
            _speed = 0;
            UIManager.Instance.UpdateSpeedFillAndTimer(_speed, 0f);
        }
    }

    // private void SetBoostAndDecreaseSpeed()
    // {
    //     if (_isSpeedBoostEnabled)
    //     {
    //         _speed = _speedBoost;
    //     }
    //     else
    //     {
    //         if (_isSpeedDecreaseEnabled)
    //         {
    //             _speed = _speedDecrease;
    //         }
    //     }
    //     UIManager.Instance.UpdateSpeedFill(_speed);
    // }

    // private void UpdateSpeedTimer()
    // {
    //     if (_isSpeedBoostEnabled)
    //     {
    //         _speedBoostDuration -= Time.deltaTime;
    //         if (_speedBoostDuration <= 0)
    //         {
    //             _isSpeedBoostEnabled = false;
    //             _speedBoostDuration = _initialSpeedBoostDuration;
    //             _speed = _originalSpeed;
    //         }
    //     }
    //     else
    //     {
    //         if (_isSpeedDecreaseEnabled)
    //         {
    //             _speedDecreaseDuration -= Time.deltaTime;
    //             if (_speedDecreaseDuration <= 0)
    //             {
    //                 _isSpeedDecreaseEnabled = false;
    //                 _speedDecreaseDuration = _initialSpeedDecreaseDuration;
    //                 _speed = _originalSpeed;
    //             }
    //         }
    //     }
    //     UIManager.Instance.UpdateSpeedFill(_speed);
    // }
}
