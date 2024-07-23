using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private string _singlePlayerScene = "GameScene";
    [SerializeField] private GameObject _loadingPanel;
    [SerializeField] private Image _loadingProgressFill;
    [SerializeField] private TextMeshProUGUI _loadingProgressText;

    private void Start()
    {
        _loadingPanel.SetActive(false);
    }

    public void OpenGameScene()
    {
        StartCoroutine("LoadGameAsync");
    }

    private IEnumerator LoadGameAsync()
    {
        string sceneName = _singlePlayerScene;

        AsyncOperation asyncLoadGame = SceneManager.LoadSceneAsync(sceneName);

        _loadingPanel.SetActive(true);
        asyncLoadGame.allowSceneActivation = false;

        while (!asyncLoadGame.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoadGame.progress / 0.9f);

            _loadingProgressFill.fillAmount = Mathf.Lerp(_loadingProgressFill.fillAmount, progress, Time.deltaTime * 5f);
            _loadingProgressText.text = $"{(progress * 100f).ToString("0")}%";

            if (asyncLoadGame.progress >= 0.9f)
            {
                asyncLoadGame.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
