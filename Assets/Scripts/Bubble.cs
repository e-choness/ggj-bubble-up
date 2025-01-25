using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Bubble : MonoBehaviour
{
    [Header("Physics")]
    [SerializeField] private float initialForce = 50f;
    [SerializeField] private Vector3 growthRate = new(0.005f, 0.005f, 1);
    [SerializeField] private Vector3 shrinkRate = new(-0.000001f, -0.000001f, 1);
    // public bool isExpanding;
    
    [Header("Properties")]
    [SerializeField] private float maxSize = 3;
    [SerializeField] private double minSize = 0.25;
    
    [Header("Controls")]
    [SerializeField] private bool isMouseDown;

    [Header("Visuals")]
    [SerializeField] private List<Sprite> sprites = new();
    
    // Components
    private SpriteRenderer _spriteRenderer;
    private CircleCollider2D _collider;
    private Rigidbody2D _rigidbody;

    //potentially use the same value for all the vector values

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<CircleCollider2D>();
        _rigidbody = GetComponent<Rigidbody2D>();
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

    private void OnMouseDrag()
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

    private void Expand()
    {
        var newSize = transform.localScale + growthRate;
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
        _rigidbody.AddForce(Vector3.up * 0.5f, ForceMode2D.Force);
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

    public void SetRandomColor()
    {
        _spriteRenderer.sprite = sprites[Random.Range(0, sprites.Count - 1)];
    }

    #endregion

    #region Physics

    public void ApplyInitialForce(Vector2 originPosition, Vector2 spawnPosition)
    {
        transform.position = spawnPosition;
        var direction = (originPosition - spawnPosition).normalized; // destination - origin
        _rigidbody.AddForce(direction * initialForce, ForceMode2D.Force);
        Debug.Log($"Applying Initial force: {direction * initialForce}");
    }

    #endregion
    
}
