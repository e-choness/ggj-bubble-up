using Game;
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
    /// Static methods are ok not using the Instance
    /// </summary>
    public class SceneController : SingletonPersistent<SceneController>
    {
        public static SceneIndex CurrentScene { get; private set; }  
        public static void LoadGameScene()
        {
            Debug.Log("Loading the game...");
            CurrentScene = SceneIndex.GameScene;
            ScoreManager.Instance.InitializeScores();
            Time.timeScale = 1; // Make sure the game is running in case you use the pause menu to exit
            
            EndGameSaves(); // Make sure to save the high scores when the player exit the game scene
            
            SceneManager.LoadScene((int)SceneIndex.GameScene);
        }

        public static void LoadMainMenu()
        {
            Debug.Log("Loading the main menu...");
            CurrentScene = SceneIndex.MainMenu;
            Time.timeScale = 1; // Make sure the game is running in case you use the pause menu to exit

            EndGameSaves(); // Make sure to save the high scores when the player exit the game scene
            
            SceneManager.LoadScene((int)SceneIndex.MainMenu);
        }

        public static void LoadGameOver()
        {
            Debug.Log("Loading the game over...");
            CurrentScene = SceneIndex.GameOver;
            SceneManager.LoadScene((int)SceneIndex.GameOver);
        }

        public static void Load(SceneIndex index)
        {
            var sceneNum = (int)index;
            CurrentScene = index;
            SceneManager.LoadScene(sceneNum);
            Debug.Log($"Loading the {index.ToString()}");
        }

        private static void EndGameSaves()
        {
            if (MainBubble.Instance != null)
            {
                MainBubble.Instance.OnGameEnd?.Invoke();
            }
        }
        
        public static void QuitGame()
        {
            Debug.Log("Quitting Game");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            EndGameSaves();
            Application.Quit();
#endif
        }

        
    }
}
