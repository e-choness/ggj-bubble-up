using UnityEngine;

public class UI : MonoBehaviour
{
    public void GoToMainMenu()
    {
        if (SceneController.Instance == null) throw new System.Exception("Missing SceneController singleton in the scene");
        SceneController.Instance.GoToMainMenu();
    }
}
