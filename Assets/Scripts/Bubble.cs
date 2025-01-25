using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class Bubble : MonoBehaviour
{    
    public new CircleCollider2D collider;
    public Rigidbody2D body;
    // public bool isExpanding;
    public Vector3 growthRate = new Vector3(0.005f, 0.005f, 1);
    public Vector3 shrinkRate = new Vector3(-0.000001f, -0.000001f, 1);
    public float maxSize = 3;
    public double minSize = 0.25;
    
    [SerializeField] private float initialForce = 5f;

    public bool isMouseDown;

    private SpriteRenderer _spriteRenderer;

    //potentially use the same value for all the vector values

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {   
    //    if (!isMouseDown) {
    //     Shrink();
    //    }
        //floatUp();
        /*
        Get current bubble size
        set velocity to negative /apply gravity

        */
    }

    #region Controls

    void OnMouseDrag()
    {
        isMouseDown = true;
        Expand();
        
    }

    void onMouseExit()
    {
        isMouseDown = false;
        Debug.Log("off bubble");
    }

    #endregion
    

    #region Movement

    void Expand()
    {
        Vector3 newSize = transform.localScale + growthRate;
        if (newSize.x < maxSize){
            transform.localScale += growthRate;//slurp this later
            //transform.localScale = Vector3.Slerp(Vector3.one, Vector3.one * maxSize, transform.localScale);//slurp this later
        }
        //add collider scaling
    }
    // void Shrink()
    // {
    //     Vector3 newSize = transform.localScale + shrinkRate;
    //     if (newSize.x > minSize){
    //         transform.localScale += shrinkRate;
    //     }
    // }
    void floatUp()
    {
        //Vector3.up
        Vector3 upwardForce = new Vector3(0f, 1f, 1);
        body.AddForce(Vector3.up * 0.5f, ForceMode2D.Force);
    }

   
    void Sink()
    {

    }
    //Expand - bubble when player clicks and holds
    //Movement - bubble rises or falls depending on the base size
    //Explode/Burst - when the bubble bursts

    #endregion
    

    #region Color

    public bool IsSameColor(Bubble other) => _spriteRenderer.sprite == other._spriteRenderer.sprite;

    public void SetRandomColor(List<Sprite> sprites)
    {
        _spriteRenderer.sprite = sprites[Random.Range(0, sprites.Count - 1)];
    }

    #endregion

    #region Physics

    public void ApplyInitialForce(Vector2 originPosition, Vector2 spawnPosition)
    {
        transform.position = spawnPosition;
        var direction = (originPosition - spawnPosition).normalized; // destination - origin
        GetComponent<Rigidbody2D>().AddForce(direction * initialForce, ForceMode2D.Force);
        Debug.Log($"Applying Initial force: {direction * initialForce}");
    }

    #endregion
    
}
