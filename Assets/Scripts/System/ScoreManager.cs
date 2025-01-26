using System.Collections;
using System.Collections.Generic;
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

        [Min(0f)] public float comboTime = 5f;

        /// <summary>
        /// An event to broadcast score changes
        /// </summary>
        public Action<int> OnCurrentScoreChanged;
        public Action<int> OnHighestScoreChanged;

        private int _currentScore;
        private int _highestScore;
        
        private const string CurrentScoreKey = "CurrentScore";
        private const string HighestScoreKey = "HighestScore";

        public int nCombos {get; private set;} = 0;

        private float comboTimer = 0f;

        public override void Awake()
        {
            base.Awake();
            InitializeScores();
        }

        void Update()
        {
            if (nCombos > 0)
            {
                comboTimer = Mathf.Min(comboTimer + Time.deltaTime, comboTime);
                if (comboTimer == comboTime)
                {
                    comboTimer = 0f;
                    ResetCombo();
                    ComboIndicator.Instance.gameObject.SetActive(false);
                }
            }
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

        public void IncrementCombo()
        {
            nCombos ++;
            comboTimer = 0f;
            ComboIndicator.Instance.gameObject.SetActive(false);
            ComboIndicator.Instance.gameObject.SetActive(true);
        } 

        public void ResetCombo() => nCombos = 0;
    }
}
