using System;
using UnityEngine;
using UnityEngine.UI;


namespace UI
{
        public class MainMenuPanel : MonoBehaviour
        {
            [SerializeField] private Button startButton;
            [SerializeField] private Button guideButton;
            [SerializeField] private Button quitButton;

            private void Start()
            {
                startButton.onClick.AddListener(SceneController.LoadGameScene);
                quitButton.onClick.AddListener(SceneController.QuitGame);
                // startButton.onClick.AddListener(ButtonClick);
                // startButton.interactable = true;
            }

            public static void ButtonClick()
            {
                Debug.Log("Button clicked.");
            }
        }
}
