using System;
using UnityEngine;
using UnityEngine.InputSystem;
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
            AssignListeners();
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
            else 
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
    }
}
