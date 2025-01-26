using System;
using System.Collections;
using Game;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UI
{
    public class PauseMenu : MonoBehaviour
    {
        [Header("Buttons")] 
        public Button resumeButton;
        [SerializeField] private Button reStartButton;
        [SerializeField] private Button guideButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button backToPauseButton;
        [SerializeField] private Button exitButton;

        [Header("Sub Panels")] 
        [SerializeField] private GameObject buttonContainer;
        [SerializeField] private GameObject guidePanel;
        [SerializeField] private  GameObject gameOverBanner;
        
        [Header("Game Over Menu")]
        [SerializeField] private int delayTime = 3;

        /// <summary>
        /// Set to false when the game is over
        /// </summary>
        [HideInInspector] public bool allowUnpause = true;

        private void Start()
        {
            AssignListeners();
        }

        private void OnEnable()
        {
            MainBubble.Instance.OnGameEnd += DisplayGameOver;
        }

        private void OnDisable()
        {
            MainBubble.Instance.OnGameEnd -= DisplayGameOver;
        }

        private void AssignListeners()
        {
            resumeButton.onClick.AddListener(TogglePause);
            reStartButton.onClick.AddListener(SceneController.LoadGameScene);
            guideButton.onClick.AddListener(() => {
                buttonContainer.SetActive(false);
                guidePanel.SetActive(true);
            });
            mainMenuButton.onClick.AddListener(SceneController.LoadMainMenu);
            backToPauseButton.onClick.AddListener(() => {
                guidePanel.SetActive(false);
                buttonContainer.SetActive(true);
            });
            exitButton.onClick.AddListener(SceneController.QuitGame);
        }

        public void TogglePause()
        {
            if (!gameObject.activeInHierarchy) // currently not paused
            {
                Time.timeScale = 0; // Stops time
                buttonContainer.SetActive(true);
                guidePanel.SetActive(false);
                gameObject.SetActive(true);
                Debug.Log("Game is now paused.");
            }
            else if (allowUnpause) 
            {
                Time.timeScale = 1;
                gameObject.SetActive(false);
                Debug.Log("Game is now resumed.");
            }
        }

        public void TogglePause(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            TogglePause();
        }

        private void DisplayGameOver()
        {
            // Delay the pop menu for [delayTime](default = 3 seconds), it can be configured on Pause Menu script
            StartCoroutine(DelayMenu());
            
            gameOverBanner.SetActive(true);
            resumeButton.interactable = false;
            allowUnpause = false;
            TogglePause();
        }

        private IEnumerator DelayMenu()
        {
            yield return new WaitForSeconds(delayTime);
        }
    }
}
