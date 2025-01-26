using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PauseMenu : MonoBehaviour
    {
        [Header("Buttons")] 
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button reStartButton;
        [SerializeField] private Button guideButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button backToPauseButton;
        [SerializeField] private Button exitButton;

        [Header("Sub Panels")] 
        [SerializeField] private GameObject buttonContainer;
        [SerializeField] private GameObject guidePanel;

        private void Start()
        {
            InitializePanels();
            AssignListeners();
        }

        private void InitializePanels()
        {
            buttonContainer.SetActive(false);
            guidePanel.SetActive(false);
        }

        private void AssignListeners()
        {
            resumeButton.onClick.AddListener(ResumeGame);
            reStartButton.onClick.AddListener(SceneController.LoadGameScene);
            guideButton.onClick.AddListener(DisplayGuide);
            mainMenuButton.onClick.AddListener(SceneController.LoadMainMenu);
            backToPauseButton.onClick.AddListener(PauseGame);
            exitButton.onClick.AddListener(SceneController.QuitGame);
        }

        private void DisplayGuide()
        {
            buttonContainer.SetActive(false);
            guidePanel.SetActive(true);
        }

        public void PauseGame()
        {
            Time.timeScale = 0; // Stops time
            gameObject.SetActive(true);
            buttonContainer.SetActive(true);
            guidePanel.SetActive(false);
            Debug.Log("Game is now paused.");
        }

        private void ResumeGame()
        {
            Time.timeScale = 1;
            InitializePanels();
            Debug.Log("Game is now resume.");
        }
    }
}
