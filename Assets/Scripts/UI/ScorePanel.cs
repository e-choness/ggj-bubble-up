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
        }

        private void OnDisable()
        {
            ScoreManager.Instance.OnCurrentScoreChanged -= DisplayCurrentScore;
            ScoreManager.Instance.OnHighestScoreChanged -= DisplayHighestScore;
        }

        private void DisplayCurrentScore(int currentSore)
        {
            currentScoreText.text = $"Current Score: {currentSore}" ;
        }

        private void DisplayHighestScore(int highestScore)
        {
            highestScoreText.text = $"Highest Score: {highestScore}" ;
        }
    }
}
