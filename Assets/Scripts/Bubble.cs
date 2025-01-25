using UnityEngine;

public class Bubble : MonoBehaviour
{
    //add bubble properties here:
    public bool isExpanding;
    private Vector3 scaleIncrement = new Vector3(0.02f, 0.02f, 1);
    public new CircleCollider2D collider;
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
    }
    void Expand()
    {

        //slurp this later
        transform.localScale += scaleIncrement;
        //add collider scaling
    }

    //Expand - bubble when player clicks and holds
    //Movement - bubble rises or falls depending on the base size
    //Explode/Burst - when the bubble bursts
}
