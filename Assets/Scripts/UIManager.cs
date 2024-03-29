using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
        // _scoreText.text = score.ToString("#,##0");
        // _highScoreText.text = $"High Score\n{highScore.ToString("#,##0")}";

        _currentScore = score;
        _scoreText.text = _currentScore.ToString("#,##0");

        _highScoreText.text = $"High Score\n{highScore.ToString("#,##0")}";
    }

    public void UpdateLives(int currentLives)
    {
        _livesImage.sprite = _livesSprites[currentLives];

        if (currentLives == 0)
        {
            GameOverSequence();
        }
    }

    private void GameOverSequence()
    {
        GameManager.Instance.GameOver();

        // int finalScore = int.Parse(_scoreText.text);

        _finalScoreText.text = $"Final Score\n{_currentScore.ToString("#,##0")}";

        _gameOverPanel.SetActive(true);

        StartCoroutine(GameOverFlickerRoutine());
    }

    private IEnumerator GameOverFlickerRoutine()
    {
        while (true)
        {
            _restartButton.gameObject.SetActive(true);
            yield return new WaitForSeconds(.5f);
            _restartButton.gameObject.SetActive(false);
            yield return new WaitForSeconds(.5f);
        }
    }
}
