using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(CircleCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Animator))]
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
        [SerializeField] private AnimationClip popAnimation;
        [SerializeField] private List<Color> colors = new();

        public UnityEvent onCollision = new();
        public UnityEvent onCollisionWithSameColor = new();
        public UnityEvent onCollisionWithDifferentColor = new();
        public UnityEvent onCollisionOutsideMainBubble = new();
        public UnityEvent onLockedInCenter = new();
        public UnityEvent onPop = new();
        
        // Components
        private SpriteRenderer _spriteRenderer;
        private CircleCollider2D _collider;
        public new CircleCollider2D collider 
        {
            get
            {
                if (_collider == null) _collider = GetComponent<CircleCollider2D>();
                return _collider;
            }
        }
        private Rigidbody2D _rigidbody;
        private AudioSource _audioSource;
        private Animator _animator;

        [HideInInspector] public bool isLocked {get; private set;}

        [HideInInspector] public List<Bubble> neighbors = new List<Bubble>();

        private bool firstCollision = true;

        //potentially use the same value for all the vector values

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _rigidbody = GetComponent<Rigidbody2D>();
            _audioSource = GetComponent<AudioSource>();
            _animator = GetComponent<Animator>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (isLocked) return;
            SetVelocity();
            LockPositionIfAtCenter();
        }
    
        public void Pop()
        {
            _animator.SetBool("isPopped", true);
            Invoke(nameof(DestroyBubble), popAnimation.length);

            foreach (Bubble neighbor in neighbors.ToArray())
            {
                if (IsSameColor(neighbor))
                {
                    Disconnect(neighbor);
                    neighbor.Pop();
                }
            }

            onPop.Invoke();
        }

        private void DestroyBubble()
        {
            Destroy(gameObject);
        }

        public void Connect(Bubble other)
        {
            if (!neighbors.Contains(other)) neighbors.Add(other);
            if (!other.neighbors.Contains(this)) other.neighbors.Add(this);
        }

        public void Disconnect(Bubble other)
        {
            if (neighbors.Contains(other)) neighbors.Remove(other);
            if (other.neighbors.Contains(this)) other.neighbors.Remove(this);
        }

        #region Color

        public bool IsSameColor(Bubble other) => _spriteRenderer.color == other._spriteRenderer.color;

        public void SetRandomColor()
        {
            _spriteRenderer.color = colors[Random.Range(0, colors.Count - 1)];
        }

        #endregion

        #region Physics

        public float GetRadius() => (transform.TransformPoint(new Vector2(collider.radius, 0f)) - transform.position).magnitude;

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
            if (!MainBubble.bubbles.Contains(this)) MainBubble.AddBubble(this);
            onLockedInCenter.Invoke();
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            onCollision.Invoke();
            Bubble other = collision.collider.GetComponent<Bubble>();
            if (other != null)
            {
                //velocity = 0f; // "sticky" behavior, no jiggling
                bool sameColor = IsSameColor(other);

                Connect(other);
                if (firstCollision) _audioSource.Play();
                
                Vector3 center = System.SpawnManager.Instance.transform.position;
                float dist = (transform.position - center).magnitude;
                if (dist + GetRadius() >= MainBubble.Instance.GetRadius())
                {
                    // Pop will trigger a chain reaction on all the neighbors as needed
                    if (sameColor) Pop();
                    else // TODO: game over
                    {
                        throw new System.Exception("Game Over");
                    }

                    onCollisionOutsideMainBubble.Invoke();
                }
                else
                {
                    if (!MainBubble.bubbles.Contains(this)) MainBubble.AddBubble(this);
                }
                
                if (sameColor) onCollisionWithSameColor.Invoke();
                else onCollisionWithDifferentColor.Invoke();
                firstCollision = false;
            }
        }

        void OnCollisionExit2D(Collision2D collision)
        {
            Bubble other = collision.collider.GetComponent<Bubble>();
            if (other != null)
            {
                Disconnect(other);
            }
        }
        #endregion

        #region Editor
        #if UNITY_EDITOR
        void OnDrawGizmos()
        {
            Color previous = Handles.color;
            Handles.color = Color.green;
            Handles.DrawWireDisc(transform.position, transform.forward, GetRadius());
            Handles.color = previous;
        }
        #endif
        #endregion
    
    }


    #if UNITY_EDITOR
    [CustomEditor(typeof(Bubble))]
    public class BubbleEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Pop")) (target as Bubble).Pop();
        }
    }
    #endif
}




