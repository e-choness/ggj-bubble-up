using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class ScrollingBackground : MonoBehaviour
{
    //backgroundImage??

     private RawImage _rawImage;
    public float scrollSpeed = 2f;//new Vector2(0.5f, 0f); // Speed of scrolling.

     private Vector2 currentOffset;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       _rawImage = GetComponent<RawImage>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 offset = _rawImage.uvRect.position + new Vector2(scrollSpeed * Time.deltaTime/4, scrollSpeed * Time.deltaTime/4);

        // offset.x %= 1;
        // offset.y %= 1;
        _rawImage.uvRect = new Rect(offset, _rawImage.uvRect.size);
    }
}
