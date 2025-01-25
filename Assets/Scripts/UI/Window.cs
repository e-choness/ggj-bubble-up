using UnityEngine;

public class Window : MonoBehaviour
{
    public void Toggle()
    {
        gameObject.SetActive(!gameObject.activeInHierarchy);
    }
}
