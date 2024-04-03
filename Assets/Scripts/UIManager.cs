using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("UIManager is NULL");
            }

            return _instance;
        }
    }

    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _highScoreText;
    [SerializeField] private Image _livesImage;
    [SerializeField] private Sprite[] _livesSprites;
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private TextMeshProUGUI _finalScoreText;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Gradient _speedColor;
    [SerializeField] private Image _speedFill;
    [SerializeField] private TextMeshProUGUI _ammoText;
    [SerializeField] private TextMeshProUGUI _speedTimerText;

    private int _currentScore;

    private void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        _gameOverPanel.SetActive(false);

        _restartButton.onClick.AddListener(() => GameManager.Instance.LoadScene());
    }

    public void UpdateScore(int score, int highScore)
    {
        _currentScore = score;
        _scoreText.text = _currentScore.ToString("#,##0");

        _highScoreText.text = $"High Score\n{highScore.ToString("#,##0")}";
    }

    public void UpdateLives(int currentLives)
    {
        if (currentLives >= 0)
        {
            _livesImage.sprite = _livesSprites[currentLives];
        }

        if (currentLives == 0)
        {
            GameOverSequence();
        }
    }

    private void GameOverSequence()
    {
        GameManager.Instance.GameOver();

        _finalScoreText.text = $"Final Score\n{_currentScore.ToString("#,##0")}";

        _gameOverPanel.SetActive(true);

        StartCoroutine(GameOverFlickerRoutine());
    }

    private IEnumerator GameOverFlickerRoutine()
    {
        float duration = .5f;
        while (true)
        {
            _restartButton.gameObject.SetActive(true);
            yield return new WaitForSeconds(duration);
            _restartButton.gameObject.SetActive(false);
            yield return new WaitForSeconds(duration);
        }
    }

    public void UpdateSpeedFillAndTimer(float currentSpeed, float timer)
    {
        _speedFill.fillAmount = Mathf.Lerp(_speedFill.fillAmount, currentSpeed / 10f, Time.deltaTime * 5f);
        _speedFill.color = SetColorSpeerBar(currentSpeed);

        _speedTimerText.text = currentSpeed switch
        {
            10f => $"High Speed Time:\n{timer.ToString("F2")}s",//   speedThreshold = 10f;
            2.5f => $"Low Speed Time:\n{timer.ToString("F2")}s",//   speedThreshold = 2.5f;
            _ => "Normal Speed",//   speedThreshold = 5f;
        };
    }

    private Color SetColorSpeerBar(float currentSpeed)
    {
        return _speedColor.Evaluate(currentSpeed / 10f);
    }

    public void UpdateAmmo(int currentAmmo, int maxAmmo)
    {
        _ammoText.text = $"Ammo:\n{currentAmmo}/{maxAmmo}";
    }

    // public void UpdateUISpeed(float currentSpeed)
    // {
    //     StartCoroutine(UpdateSpeedRoutine(currentSpeed));
    // }

    // private IEnumerator UpdateSpeedRoutine(float targetSpeed)
    // {
    //     float startSpeed = _speedSlider.value;

    //     float elapsed = targetSpeed / 10f - startSpeed;
    //     float duration = .5f;

    //     while (Mathf.Abs(elapsed) > 0.001f)
    //     {
    //         _speedSlider.value += elapsed * Time.deltaTime;
    //         _speedSliderFill.color = _speedColor.Evaluate(_speedSlider.value);
    //         elapsed -= elapsed * Time.deltaTime / duration;
    //         yield return null;
    //     }

    //     _speedSlider.value = targetSpeed / 10f;
    //     _speedSliderFill.color = _speedColor.Evaluate(targetSpeed / 10f);
    // }
}
