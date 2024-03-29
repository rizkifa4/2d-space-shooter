using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("GameManager is NULL");
            }

            return _instance;
        }
    }

    private bool _hasGameOver;
    private string _gameScene = "GameScene";
    private void Awake()
    {
        _instance = this;
    }

    private void Update()
    {
        if (_hasGameOver && Input.GetKeyDown(KeyCode.R))
        {
            LoadScene();
        }
    }

    public void LoadScene()
    {
        SceneManager.LoadScene(_gameScene);
    }

    public void GameOver()
    {
        _hasGameOver = true;
    }
}
