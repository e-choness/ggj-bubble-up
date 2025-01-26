using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartText : MonoBehaviour
{
    
    public TextMeshProUGUI startText;
    public float displayDuration = 1f;
    public float bounceDuration = 0.5f;
    public Vector3 startScale = Vector3.zero;
    public Vector3 targetScale = Vector3.one;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    void Awake()
    {
        startText = GetComponent<TextMeshProUGUI>();
    }
    void Start()
    {
        startText.text = "Start Popping!";
        startText.transform.localScale = startScale;
        StartCoroutine(BounceAnimation());
    }

    private IEnumerator BounceAnimation()
    {
        float elapsedTime = 0f;
        
        // Bounce in
        while (elapsedTime < bounceDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / bounceDuration;
            startText.transform.localScale = Vector3.Lerp(startScale, targetScale * 1.2f, progress);
            yield return null;
        }

        elapsedTime = 0f;

        while (elapsedTime < bounceDuration / 2f)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / (bounceDuration / 2f);
            startText.transform.localScale = Vector3.Lerp(targetScale * 1.2f, targetScale, progress);
            yield return null;
        }

        //Wait to hide text
        yield return new WaitForSeconds(displayDuration);
        startText.text = "";
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
