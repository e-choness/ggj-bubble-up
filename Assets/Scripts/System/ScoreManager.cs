using UnityEngine;

namespace System
{
    public class ScoreManager : SingletonPersistent<ScoreManager>
    {
        /// <summary>
        /// An event to broadcast score changes
        /// </summary>
        public Action<int> OnScoreChanged;

        private int _currentScore;
        private int _highestScore;
        
        private const string CurrentScoreKey = "CurrentScore";
        private const string HighestScoreKey = "HighestScore";

        private void Start()
        {
            InitializeScores();
        }

        private void InitializeScores()
        {
            // On the current game start the score should be 0
            _currentScore = 0;
            
            // Load the players previous highest score
            _highestScore = LoadScore(HighestScoreKey);
        }

        public void CalculateScore(int change)
        {
            _currentScore += change;
            
            if(_currentScore < 0)
                _currentScore = 0;
            
            if(_currentScore > _highestScore)
                _highestScore = _currentScore;
            
            OnScoreChanged?.Invoke(_currentScore);
        }
        
        public void EndGameScores()
        {
            SaveScore(CurrentScoreKey);
            SaveScore(HighestScoreKey);
        }

        private void SaveScore(string scoreKey)
        {
            PlayerPrefs.SetInt(scoreKey, _currentScore);
        }

        private static int LoadScore(string scoreKey)
        {
            return PlayerPrefs.GetInt(scoreKey);
        }
    }
}
