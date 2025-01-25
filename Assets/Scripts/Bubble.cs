using UnityEngine;

public class Bubble : MonoBehaviour
{
    //add bubble properties here:
    public bool isExpanding;
    private Vector3 scaleIncrement = new Vector3(0.005f, 0.005f, 1);
    private float maxSize = 3;
    public new CircleCollider2D collider;
    public Rigidbody2D body;
    //potentially use the same value for all the vector values

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    void Awake()
    {
     
    }

    // Update is called once per frame
    void Update()
    {

        /*
        Get current bubble size
        set velocity to negative /apply gravity

        */
    }

    void OnMouseDrag()
    {
        Expand();
        floatUp();
    }
    void Expand()
    {
        Vector3 newSize = transform.localScale += scaleIncrement;
        Debug.Log(newSize.x);
        if (newSize.x < maxSize){
        transform.localScale += scaleIncrement;//slurp this later
        }
        //add collider scaling
    }

    void floatUp()
    {
        Vector3 upwardForce = new Vector3(0f, 1f, 1);
        body.AddForce(upwardForce, ForceMode2D.Force);
    }

    void Sink()
    {

    }
    //Expand - bubble when player clicks and holds
    //Movement - bubble rises or falls depending on the base size
    //Explode/Burst - when the bubble bursts
}
