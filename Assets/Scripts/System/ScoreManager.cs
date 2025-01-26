using System.Collections;
using Game;
using UnityEngine;

namespace System
{
    public class ScoreManager : SingletonPersistent<ScoreManager>
    {
        public int scorePerPop = 1;
        /// <summary>
        /// After the first combo, all pops are multiplied by this value. On subsequent combos before the combo timer runs out, this 
        /// multiplier increases by += comboMultiplier
        /// </summary>
        public int comboMultiplier = 2;

        /// <summary>
        /// An event to broadcast score changes
        /// </summary>
        public Action<int> OnCurrentScoreChanged;
        public Action<int> OnHighestScoreChanged;

        private int _currentScore;
        private int _highestScore;
        
        private const string CurrentScoreKey = "CurrentScore";
        private const string HighestScoreKey = "HighestScore";

        private int nCombos = 0;

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


        public void ProcessBubblePop(Bubble bubble)
        {
            if (nCombos == 0) CalculateScore(scorePerPop);
            else CalculateScore(nCombos * comboMultiplier * scorePerPop);
        }

        public void IncrementCombo() => nCombos += 1;

        public void ResetCombo() => nCombos = 0;
    }
}
