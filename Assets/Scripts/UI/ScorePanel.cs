using System;
using TMPro;
using UnityEngine;

namespace UI
{
    public class ScorePanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text currentScoreText;
        [SerializeField] private TMP_Text highestScoreText;
        
        /// <summary>
        /// Needs to be disabled during main menu
        /// </summary>
        [SerializeField] private GameObject currentScoreBackground;

        private void Start()
        {
            
            ScoreManager.Instance.OnCurrentScoreChanged += DisplayCurrentScore;
            ScoreManager.Instance.OnHighestScoreChanged += DisplayHighestScore;

            Debug.Log($"Current scene: {SceneController.CurrentScene}");
            currentScoreBackground.SetActive(SceneController.CurrentScene != SceneIndex.MainMenu);
        }

        private void OnDisable()
        {
            ScoreManager.Instance.OnCurrentScoreChanged -= DisplayCurrentScore;
            ScoreManager.Instance.OnHighestScoreChanged -= DisplayHighestScore;
        }

        private void DisplayCurrentScore(int currentSore)
        {
            currentScoreText.text = $"Current score: {currentSore}";
        }

        private void DisplayHighestScore(int highestScore)
        {
            highestScoreText.text = $"Highest Score: {highestScore}" ;
        }
    }
}
