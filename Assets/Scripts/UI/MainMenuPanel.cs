using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
        public class MainMenuPanel : MonoBehaviour
        {
            [Header("Buttons")]
            [SerializeField] private Button startButton;
            [SerializeField] private Button guideButton;
            [SerializeField] private Button quitButton;
            [SerializeField] private Button backToMainButton;

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
                buttonContainer.SetActive(true);
                guidePanel.SetActive(false);
            }
            private void AssignListeners()
            {
                startButton.onClick.AddListener(SceneController.LoadGameScene);
                guideButton.onClick.AddListener(DisplayGuide);
                quitButton.onClick.AddListener(SceneController.QuitGame);
                backToMainButton.onClick.AddListener(InitializePanels);
            }

            private void DisplayGuide()
            {
                buttonContainer.SetActive(false);
                guidePanel.SetActive(true);
            }
        }
}
