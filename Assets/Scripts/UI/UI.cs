using System;
using UnityEngine;

namespace UI
{
    public class UI : MonoBehaviour
    {
        public void GoToMainMenu()
        {
            if (SceneController.Instance == null) throw new Exception("Missing SceneController singleton in the scene");
            SceneController.Instance.GoToMainMenu();
        }
    }
}
