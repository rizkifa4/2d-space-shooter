using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class Options : MonoBehaviour
{

    [SerializeField] private GameObject _hudDisplay;
    [SerializeField] private GameObject _pauseMenuPanel;
    [SerializeField] private GameObject _pauseMenuContainer;
    [SerializeField] private GameObject _exitConfirmationContainer;
    [SerializeField] private GameObject _mainMenuConfirmationContainer;
    private RuntimePlatform _platform = RuntimePlatform.WebGLPlayer;
    private bool _isPaused;
    private bool _isExitConfirmation;
    private bool _isMainMenuConfirmation;
    private Animator _pauseMenuAnimator;
    private Animator _hudAnimator;
    private bool _isPausedAnimRunning;
    private bool _isResumeAnimRunning;
    [SerializeField] private Button _resumeButton;
    private Button _bgResumeButton;

    [DllImport("__Internal")]
    private static extern void CloseWindow();

    private void Start()
    {
        _pauseMenuAnimator = _pauseMenuPanel.GetComponent<Animator>();
        _hudAnimator = _hudDisplay.GetComponent<Animator>();

        _pauseMenuPanel.SetActive(true);
        _pauseMenuContainer.SetActive(false);
        _mainMenuConfirmationContainer.SetActive(false);
        _exitConfirmationContainer.SetActive(false);

        _bgResumeButton = _pauseMenuContainer.GetComponent<Button>();
    }

    public bool IsPausedAnimRunning
    {
        get { return _isPausedAnimRunning; }
        set { _isPausedAnimRunning = value; }
    }

    public bool IsResumeAnimRunning
    {
        get { return _isResumeAnimRunning; }
        set { _isResumeAnimRunning = value; }
    }

    public Button ResumeButton
    {
        get { return _resumeButton; }
    }

    public Button BgResumeButton
    {
        get { return _bgResumeButton; }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_isPaused)
            {
                if (_isExitConfirmation)
                {
                    ExitGameCancel();
                }
                else if (_isMainMenuConfirmation)
                {
                    MainMenuCancel();
                }
                else
                {
                    ResumeGame();
                }
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        if (!_isResumeAnimRunning)
        {
            Time.timeScale = 0;
            _isPaused = true;

            _pauseMenuAnimator.SetTrigger("PauseIn");
            _hudAnimator.SetTrigger("Hud_PauseIn");
        }
    }

    public void ResumeGame()
    {
        if (!_isPausedAnimRunning)
        {
            _isPaused = false;

            _pauseMenuAnimator.SetTrigger("PauseOut");
            _hudAnimator.SetTrigger("Hud_PauseOut");

            _resumeButton.interactable = false;
            _bgResumeButton.interactable = false;
        }
    }

    public void Back2MainMenuOpen()
    {
        _isMainMenuConfirmation = true;

        _pauseMenuAnimator.SetTrigger("MainMenuIn");
    }

    public void MainMenuCancel()
    {
        _isMainMenuConfirmation = false;

        _pauseMenuAnimator.SetTrigger("MainMenuOut");
    }

    public void MainMenuConfirm()
    {
        Time.timeScale = 1;

        GameManager.Instance.BackToMainMenu();
    }

    public void ExitGameOpen()
    {
        _isExitConfirmation = true;

        _pauseMenuAnimator.SetTrigger("ExitGameConfirmIn");
    }

    public void ExitGameCancel()
    {
        _isExitConfirmation = false;

        _pauseMenuAnimator.SetTrigger("ExitGameConfirmOut");
    }

    public void ExitGameConfirm()
    {
        if (CheckForWebGL()) CloseWindow(); else Application.Quit();
    }

    private bool CheckForWebGL()
    {
        return Application.platform == _platform;
    }
}
