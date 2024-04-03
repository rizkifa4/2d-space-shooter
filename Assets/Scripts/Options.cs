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
    private RuntimePlatform _platform = RuntimePlatform.WebGLPlayer;
    private bool _isPaused;
    private bool _isExitConfirmation;

    [DllImport("__Internal")]
    private static extern void CloseWindow();
    void Start()
    {
        _pauseMenuContainer.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_isPaused)
            {
                if (_isExitConfirmation)
                {
                    ExitGameCancel();
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
        _isPaused = true;
        _pauseMenuContainer.SetActive(true);
        _pauseMenuContainer.SetActive(true);
        _hudDisplay.SetActive(false);
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        _isPaused = false;
        _pauseMenuContainer.SetActive(false);
        _hudDisplay.SetActive(true);
        Time.timeScale = 1;
    }

    public void ExitGameOpen()
    {
        _isExitConfirmation = true;
        _pauseMenuContainer.SetActive(false);
        _exitConfirmationContainer.SetActive(true);
    }

    public void ExitGameCancel()
    {
        _isExitConfirmation = false;
        _exitConfirmationContainer.SetActive(false);
        _pauseMenuContainer.SetActive(true);
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
