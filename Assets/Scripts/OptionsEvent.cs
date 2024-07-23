using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsEvent : MonoBehaviour
{
    private Options _options;

    private void Start()
    {
        _options = GameObject.FindGameObjectWithTag("UIManager").GetComponent<Options>();
    }

    private void StartEventPauseIn()
    {
        _options.IsPausedAnimRunning = true;
    }

    private void ExitEventPauseIn()
    {
        _options.IsPausedAnimRunning = false;
    }

    private void StartEventPauseOut()
    {
        _options.IsResumeAnimRunning = true;
    }

    private void ExitEventPauseOut()
    {
        _options.IsResumeAnimRunning = false;
        _options.ResumeButton.interactable = true;
        _options.BgResumeButton.interactable = true;

        Time.timeScale = 1;
    }
}
