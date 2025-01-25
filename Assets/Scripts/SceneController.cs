using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

/// <summary>
/// Singleton that controls scene changes. Use the Instance field.
/// </summary>
public class SceneController : MonoBehaviour
{
    [SerializeField] private string mainMenuName;
    [SerializeField] private string gameSceneName;

    public UnityEvent onGoToMainMenu = new UnityEvent();
    public UnityEvent onGoToGameScene = new UnityEvent();

    public static SceneController Instance;

    void Awake()
    {
        // Singleton behavior
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void GoToGameScene()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(mainMenuName);
    }
}
