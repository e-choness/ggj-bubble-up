using System;
using TMPro;
using UnityEngine;

namespace UI
{
    public class ScorePanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text currentScoreText;
        [SerializeField] private TMP_Text highestScoreText;

        private void Start()
        {
            
            ScoreManager.Instance.OnCurrentScoreChanged += DisplayCurrentScore;
            ScoreManager.Instance.OnHighestScoreChanged += DisplayHighestScore;

            if (SceneController.CurrentScene == SceneIndex.MainMenu)
                currentScoreText.text = "";
        }

        private void OnDisable()
        {
            ScoreManager.Instance.OnCurrentScoreChanged -= DisplayCurrentScore;
            ScoreManager.Instance.OnHighestScoreChanged -= DisplayHighestScore;
        }

        private void DisplayCurrentScore(int currentSore)
        {
            Debug.Log($"Current scene: {SceneController.CurrentScene}");
            currentScoreText.text = SceneController.CurrentScene == SceneIndex.MainMenu ? 
                "" : $"Current Score: {currentSore}";
        }

        private void DisplayHighestScore(int highestScore)
        {
            highestScoreText.text = $"Highest Score: {highestScore}" ;
        }
    }
}
