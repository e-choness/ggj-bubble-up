using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(CircleCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class Bubble : MonoBehaviour
    {
        [Header("Physics")]
        [SerializeField] private float velocity = 50f;
        [SerializeField] private Vector3 growthRate = new(0.005f, 0.005f, 1);
        [SerializeField] private Vector3 shrinkRate = new(-0.000001f, -0.000001f, 1);

        /// <summary>
        /// When the bubble gets close to the center of the screen, it should "snap" into place and never move thereafter until it is popped.
        /// This field is the fraction of the bubble's radius by which it will "snap", where 0 means the bubble has to be positioned exactly
        /// at the center, and 1 means the bubble will snap into place when it is less than 1 radius away.
        /// </summary>
        [SerializeField, Range(0f, 1f)] private float centralLockingFactor = 0.5f;
        // public bool isExpanding;
    
        [Header("Properties")]
        [SerializeField] private float maxSize = 3;
        [SerializeField] private double minSize = 0.25;
    
        [Header("Controls")]
        [SerializeField] private bool isMouseDown;

        [Header("Visuals")]
        [SerializeField] private List<Color> colors = new();

        public UnityEvent onCollision = new();
        public UnityEvent onCollisionWithSameColor = new();
        public UnityEvent onCollisionWithDifferentColor = new();
        public UnityEvent onLockedInCenter = new();
    
        // Components
        private SpriteRenderer _spriteRenderer;
        private CircleCollider2D _collider;
        private Rigidbody2D _rigidbody;

        [HideInInspector] public bool isLocked {get; private set;}

        //potentially use the same value for all the vector values

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _collider = GetComponent<CircleCollider2D>();
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (isLocked) return;
            SetVelocity();
            LockPositionIfAtCenter();
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

        public bool IsSameColor(Bubble other) => _spriteRenderer.color == other._spriteRenderer.color;

        public void SetRandomColor()
        {
            _spriteRenderer.color = colors[Random.Range(0, colors.Count - 1)];
        }

        #endregion

        #region Physics

        private void SetVelocity()
        {
            Vector3 direction = (System.SpawnManager.Instance.transform.position - transform.position).normalized; // destination - origin
            _rigidbody.linearVelocity = direction * velocity;
        }

        private void LockPositionIfAtCenter()
        {
            Vector3 target = System.SpawnManager.Instance.transform.position;
            if ((transform.position - target).sqrMagnitude > centralLockingFactor * centralLockingFactor) return;
            //velocity = 0f;
            transform.position = target;
            _rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
            isLocked = true;
            onLockedInCenter.Invoke();
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            onCollision.Invoke();
            Bubble other = collision.collider.GetComponent<Bubble>();
            if (other != null)
            {
                velocity = 0f;
                if (IsSameColor(other)) onCollisionWithSameColor.Invoke();
                else onCollisionWithDifferentColor.Invoke();
            }
        }
        #endregion
    
    }
}
