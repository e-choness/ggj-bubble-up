using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace System
{
    /// <summary>
    /// Singleton that controls scene changes. Use the Instance field.
    /// </summary>
    public class SceneController : SingletonPersistent<SceneController>
    {
        [SerializeField] private string mainMenuName;
        [SerializeField] private string gameSceneName;
        [SerializeField] private string gameOverScene;

        public UnityEvent onGoToMainMenu = new();
        public UnityEvent onGoToGameScene = new();
        public UnityEvent onGoToGameOver = new();

        public void GoToGameScene()
        {
            SceneManager.LoadScene(gameSceneName);
            onGoToGameScene.Invoke();
        }

        public void GoToMainMenu()
        {
            SceneManager.LoadScene(mainMenuName);
            onGoToMainMenu.Invoke();
        }

        public void GoToGameOver()
        {
            SceneManager.LoadScene(gameOverScene);
            onGoToGameOver.Invoke();
        }
    }
}
