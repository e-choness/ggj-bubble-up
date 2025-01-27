using UnityEngine;
using UnityEngine.Events;

public class CollisionEvents : MonoBehaviour
{
    public UnityEvent onCollisionEnter;
    public UnityEvent onCollisionExit;
    public UnityEvent onCollisionStay;

    public UnityEvent onCollisionEnter2D = new();
    public UnityEvent onCollisionExit2D = new();
    public UnityEvent onCollisionStay2D = new();

    public UnityEvent onParticleCollision = new();
    public UnityEvent onControllerColliderHit = new();

    private void OnCollisionEnter(Collision collision) => onCollisionEnter.Invoke();
    private void OnCollisionExit(Collision collision) => onCollisionExit.Invoke();
    private void OnCollisionStay(Collision collision) => onCollisionStay.Invoke();

    private void OnCollisionEnter2D(Collision2D collision) => onCollisionEnter2D.Invoke();
    private void OnCollisionExit2D(Collision2D collision) => onCollisionExit2D.Invoke();
    private void OnCollisionStay2D(Collision2D collision) => onCollisionStay2D.Invoke();

    private void OnParticleCollision(GameObject other) => onParticleCollision.Invoke();
    private void OnControllerColliderHit(ControllerColliderHit hit) => onControllerColliderHit.Invoke();
}
