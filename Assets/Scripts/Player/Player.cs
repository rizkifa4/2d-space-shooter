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
    private SpawnManager _spawnManager;
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
    private Animator _playerAnim;
    [SerializeField] private int _laserAmmo;
    [SerializeField] private int _maxAmmo;
    private bool _isTurnRight;
    private bool _isDead;
    private SoundManager _soundManager;

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

    private void Start()
    {
        _bulletSpawnPoint = transform.GetChild(0);

        _spawnManager = FindObjectOfType<SpawnManager>();

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

        _soundManager = GameObject.FindWithTag("SoundManager").GetComponent<SoundManager>();
    }

    private void Update()
    {
        Movement();

        LaserShoot();

        SetBoostAndDecreaseSpeedTimer();
    }

    private void Movement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        transform.Translate(_speed * Time.deltaTime * direction);

        float xRange = 11;
        float yMin = -4;
        float yMax = 0;

        float ClampY = Mathf.Clamp(transform.position.y, yMin, yMax);
        float ClampX = transform.position.x;

        if (ClampX < -xRange)
        {
            ClampX = xRange;
        }
        else if (ClampX > xRange)
        {
            ClampX = -xRange;
        }

        transform.position = new Vector3(ClampX, ClampY, transform.position.z);

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
        else
        {
            _playerAnim.SetBool(_isTurnRight ? "TurnRight" : "TurnLeft", false);
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
            _nextFire = Time.time + _fireRate;

            GameObject bullet = !_isTripleShotEnabled ? _laserPrefab : _tripleShotPrefab;
            Instantiate(bullet, _bulletSpawnPoint.position, Quaternion.identity);

            _laserAmmo--;
            UIManager.Instance.UpdateAmmo(_laserAmmo, _maxAmmo);

            _soundManager.PlaySoundEffect(_soundManager.PlayerLaserSound);
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
                _soundManager.PlaySoundEffect(_soundManager.ExplosionSound);
                Destroy(GetComponent<Collider2D>());

                _isDead = true;
                float destroyDelay = 2.5f;
                Destroy(gameObject, destroyDelay);
            }

            _soundManager.PlaySoundEffect(_soundManager.ExplosionSound);
        }
    }

    public void TripleShotActive()
    {
        _isTripleShotEnabled = true;

        _soundManager.PlaySoundEffect(_soundManager.PositivePowerUpSound);

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

        _soundManager.PlaySoundEffect(_soundManager.PositivePowerUpSound);
    }

    public void SpeedDecreasePowerupActive()
    {
        _isSpeedBoostEnabled = false;

        _speedDecreaseDuration = _initialSpeedDecreaseDuration;
        _speed = _speedDecrease;

        _isSpeedDecreaseEnabled = true;

        _soundManager.PlaySoundEffect(_soundManager.NegativePowerUpSound);
    }

    public void ShieldPowerupActive()
    {
        _isShieldEnabled = true;
        _playerShield.SetActive(true);

        _soundManager.PlaySoundEffect(_soundManager.PositivePowerUpSound);

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

        _soundManager.PlaySoundEffect(_soundManager.PositivePowerUpSound);
    }

    public void RestoreAmmo(int ammo)
    {
        _laserAmmo += ammo;

        if (_laserAmmo > _maxAmmo)
        {
            _laserAmmo = _maxAmmo;
        }

        UIManager.Instance.UpdateAmmo(_laserAmmo, _maxAmmo);

        _soundManager.PlaySoundEffect(_soundManager.PositivePowerUpSound);
    }

    private void SetBoostAndDecreaseSpeedTimer()
    {
        if (_isDead)
        {
            _speed = 0;
            UIManager.Instance.UpdateSpeedFillAndTimer(_speed, 0f);
            return;
        }

        if (_isSpeedBoostEnabled)
        {
            _speedBoostDuration -= Time.deltaTime;

            if (_speedBoostDuration <= 0f)
            {
                _isSpeedBoostEnabled = false;
                _speed = _originalSpeed;
            }
        }
        else if (_isSpeedDecreaseEnabled)
        {
            _speedDecreaseDuration -= Time.deltaTime;

            if (_speedDecreaseDuration <= 0f)
            {
                _isSpeedDecreaseEnabled = false;
                _speed = _originalSpeed;
            }
        }
        else
        {
            _speed = _originalSpeed;
        }

        UIManager.Instance.UpdateSpeedFillAndTimer(_speed,
        _isSpeedBoostEnabled ? _speedBoostDuration :
        (_isSpeedDecreaseEnabled ? _speedDecreaseDuration : 0f));
    }
}
