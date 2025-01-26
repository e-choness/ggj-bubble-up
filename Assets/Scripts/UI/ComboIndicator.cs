using System.Collections;
using UnityEngine;


public class ComboIndicator : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text comboNumber;
    [SerializeField] private float rotationOffset = 1f;
    [SerializeField] private AnimationCurve zoom;

    private Quaternion originalRotation;
    private Vector2 originalScale;

    public static ComboIndicator Instance;

    void Awake()
    {
        originalScale = transform.localScale;
        originalRotation = transform.rotation;
        Instance = this; // voodoo garbage
        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        Vector3 current = transform.rotation.eulerAngles;
        current.z += Random.Range(-rotationOffset, rotationOffset);
        transform.rotation = Quaternion.Euler(current);
        comboNumber.SetText("x" + System.ScoreManager.Instance.nCombos);
        StartCoroutine(DoZoom());
    }

    void OnDisable()
    {
        transform.rotation = originalRotation;
    }

    private IEnumerator DoZoom()
    {
        float startTime = Time.timeSinceLevelLoad;
        float timeLimit = zoom.keys[zoom.keys.Length - 1].time;
        float currentTime = 0f;
        transform.localScale = originalScale;
        while (currentTime <= timeLimit)
        {
            transform.localScale = originalScale * zoom.Evaluate(currentTime);
            yield return null;
            currentTime = Time.timeSinceLevelLoad - startTime;
        }
        transform.localScale = originalScale;
    }
}