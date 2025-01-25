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

        /// <summary>
        /// When the bubble gets close to the center of the screen, it should "snap" into place and never move thereafter until it is popped.
        /// This field is the fraction of the bubble's radius by which it will "snap", where 0 means the bubble has to be positioned exactly
        /// at the center, and 1 means the bubble will snap into place when it is less than 1 radius away.
        /// </summary>
        [SerializeField, Range(0f, 1f)] private float centralLockingFactor = 0.5f;

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
    

        #region Color

        public bool IsSameColor(Bubble other) => _spriteRenderer.color == other._spriteRenderer.color;

        public void SetRandomColor()
        {
            _spriteRenderer.color = colors[Random.Range(0, colors.Count - 1)];
        }

        #endregion

        #region Physics

        public float GetRadius() => _collider.radius;

        private void SetVelocity()
        {
            Vector3 direction = (System.SpawnManager.Instance.transform.position - transform.position).normalized; // destination - origin
            _rigidbody.linearVelocity = direction * velocity;
        }

        private void LockPositionIfAtCenter()
        {
            Vector3 target = System.SpawnManager.Instance.transform.position;
            if ((transform.position - target).magnitude > GetRadius() * centralLockingFactor) return;
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
                velocity = 0f; // "sticky" behavior, no jiggling
                if (IsSameColor(other)) onCollisionWithSameColor.Invoke();
                else onCollisionWithDifferentColor.Invoke();
            }
        }
        #endregion
    
    }
}
