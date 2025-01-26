using System.Collections;
using UnityEngine;

namespace System
{
    public class ScoreManager : SingletonPersistent<ScoreManager>
    {
        /// <summary>
        /// An event to broadcast score changes
        /// </summary>
        public Action<int> OnCurrentScoreChanged;
        public Action<int> OnHighestScoreChanged;

        private int _currentScore;
        private int _highestScore;
        
        private const string CurrentScoreKey = "CurrentScore";
        private const string HighestScoreKey = "HighestScore";

        public override void Awake()
        {
            base.Awake();
            InitializeScores();
        }

        public void InitializeScores()
        {
            // On the current game start the score should be 0
            _currentScore = 0;
            OnCurrentScoreChanged?.Invoke(_currentScore);
            
            // Load the players previous highest score
            _highestScore = LoadScore(HighestScoreKey);
            OnHighestScoreChanged?.Invoke(_highestScore);
        }

        public void CalculateScore(int change)
        {
            _currentScore += change;
            
            if(_currentScore < 0)
                _currentScore = 0;
            
            OnCurrentScoreChanged?.Invoke(_currentScore);

            if (_currentScore > _highestScore)
            {
                _highestScore = _currentScore;
                OnHighestScoreChanged?.Invoke(_highestScore);
            }
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
            return PlayerPrefs.GetInt(scoreKey, 0);
        }

        #region Test - delete later

        private void Start()
        {
            // StartCoroutine(IncrementScore());
        }

        private IEnumerator IncrementScore()
        {
            for (var i = 0; i < 100; i++)
            {
                CalculateScore(1);
                Debug.Log($"Current score: {_currentScore}");
                yield return new WaitForSeconds(1);
            }
        }

        #endregion
    }
}
