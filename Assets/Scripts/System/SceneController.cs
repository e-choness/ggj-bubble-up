using UnityEngine;
using UnityEngine.SceneManagement;

namespace System
{
    /// <summary>
    /// Look up to the Files -> Build Profiles for Scene Indexes
    /// If anything changes please change accordingly
    /// </summary>
    public enum SceneIndex
    {
        MainMenu = 0,
        GameScene = 1,
        GameOver = 2
    }
    
    /// <summary>
    /// Singleton that controls scene changes. Use the Instance field.
    /// </summary>
    public class SceneController : SingletonPersistent<SceneController>
    {
        public static void LoadGameScene()
        {
            Debug.Log("Loading the game...");
            SceneManager.LoadScene((int)SceneIndex.GameScene);
        }

        public static void LoadMainMenu()
        {
            Debug.Log($"Loading the main menu...");
            SceneManager.LoadScene((int)SceneIndex.MainMenu);
        }

        public static void LoadGameOver()
        {
            Debug.Log($"Loading the game over...");
            SceneManager.LoadScene((int)SceneIndex.GameOver);
        }

        public static void QuitGame()
        {
            Debug.Log("Quitting Game");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
