using System;
using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] private float coyoteTime = 0.1f;

        [Header("Physics")]
        [SerializeField, Min(0f)] private float initialVelocity = 50f;
        [SerializeField, Min(0f)] private float finalVelocity = 40f;

        /// <summary>
        /// When the bubble gets close to the center of the screen, it should "snap" into place and never move thereafter until it is popped.
        /// This field is the fraction of the bubble's radius by which it will "snap", where 0 means the bubble has to be positioned exactly
        /// at the center, and 1 means the bubble will snap into place when it is less than 1 radius away.
        /// </summary>
        [SerializeField, Range(0f, 1f)] private float centralLockingFactor = 0.5f;

        [Header("Visuals")]
        [SerializeField] private AnimationClip popAnimation;
        public List<Color> colors = new();
        [SerializeField] private SpriteRenderer glowEffect;

        #region Unity Events
        public UnityEvent onCollision = new();
        public UnityEvent onCollisionWithSameColor = new();
        public UnityEvent onCollisionWithDifferentColor = new();
        public UnityEvent onCollisionOutsideMainBubble = new();
        public UnityEvent onFirstCollision = new();
        public UnityEvent onReachedCenter = new();
        public UnityEvent onPop = new();
        public UnityEvent onBeforeCombo = new();
        public UnityEvent onAfterCombo = new();
        #endregion

        #region References
        private SpriteRenderer _spriteRenderer;
        [HideInInspector] public SpriteRenderer spriteRenderer
        {
            get
            {
                if (_spriteRenderer == null) _spriteRenderer = GetComponent<SpriteRenderer>();
                return _spriteRenderer;
            }
        }
        private CircleCollider2D _collider;
        [HideInInspector] public new CircleCollider2D collider 
        {
            get
            {
                if (_collider == null) _collider = GetComponent<CircleCollider2D>();
                return _collider;
            }
        }
        private Rigidbody2D _rigidbody;
        [HideInInspector] public Rigidbody2D rigidbody
        {
            get
            {
                if (_rigidbody == null) _rigidbody = GetComponent<Rigidbody2D>();
                return _rigidbody;
            }
        }
        private Animator _animator;
        [HideInInspector] public Animator animator
        {
            get
            {
                if (_animator == null) _animator = GetComponent<Animator>();
                return _animator;
            }
        }
        #endregion

        [HideInInspector] public bool isLocked {get; private set;}

        [HideInInspector] public List<Bubble> neighbors = new();

        [HideInInspector] public bool firstCollision = true;

        private Sprite originalSprite;

        //potentially use the same value for all the vector values

        void Awake()
        {
            originalSprite = spriteRenderer.sprite;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (!isLocked) SetVelocity();
            if (IsAtCenter()) OnReachCenter();
        }

        void Reset()
        {
            spriteRenderer.sprite = originalSprite;
            animator.SetBool("isPopped", false);
            if (glowEffect != null) glowEffect.enabled = true;
            if (isLocked) UnlockPosition();
            neighbors.Clear();
            firstCollision = true;
        }
    
        public void Pop()
        {
            animator.SetBool("isPopped", true);
            if (glowEffect != null) glowEffect.enabled = false;
            Invoke(nameof(DestroyBubble), popAnimation.length);
            
            MainBubble.Instance.bubblesPoppedThisFrame.Add(this);

            foreach (Bubble neighbor in neighbors.ToArray())
            {
                if (IsSameColor(neighbor))
                {
                    Disconnect(neighbor);
                    neighbor.Pop();
                }
            }

            if (MainBubble.centralBubble == this) MainBubble.centralBubble = null;

            System.ScoreManager.Instance.ProcessBubblePop(this);

            onPop.Invoke();
        }

        private void DestroyBubble()
        {
            Reset();
            System.SpawnManager.Instance.ReturnToPool(this);
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

        public bool IsSameColor(Bubble other) => GetColor() == other.GetColor();

        public void SetRandomColor() => SetColor(colors[UnityEngine.Random.Range(0, colors.Count - 1)]);

        public void SetColor(Color color) 
        {
            spriteRenderer.color = color;
        }

        public Color GetColor() => _spriteRenderer.color;

        #endregion

        #region Physics

        public float GetRadius() => (transform.TransformPoint(new Vector2(collider.radius, 0f)) - transform.position).magnitude;

        private float GetVelocity()
        {
            float R = MainBubble.Instance.GetRadius();
            float distToMainBubble = (transform.position - MainBubble.Instance.transform.position).magnitude;
            float startRadius = R * MainBubble.Instance.velocityRadiusFactor;
            float q = distToMainBubble - R;
            float d = startRadius - R;
            float progress = Mathf.Clamp(1f - (q - R) / d, 0f, 1f);
            return Mathf.Lerp(initialVelocity, finalVelocity, progress);
        }

        private void SetVelocity()
        {
            Vector3 direction = (SpawnManager.Instance.transform.position - transform.position).normalized; // destination - origin
            rigidbody.linearVelocity = direction * GetVelocity();
        }

        private bool IsAtCenter()
        {
            Vector3 target = SpawnManager.Instance.transform.position;
            return (transform.position - target).magnitude <= GetRadius() * centralLockingFactor;
        }

        private void OnReachCenter()
        {
            LockPosition();
            if (!MainBubble.bubbles.Contains(this)) MainBubble.AddBubble(this);
            if (MainBubble.centralBubble != this)
            {
                MainBubble.centralBubble = this;
                ProcessCombo();
            }
            onReachedCenter.Invoke();
        }

        private void LockPosition()
        {
            transform.position = SpawnManager.Instance.transform.position;
            rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
            isLocked = true;
        }
        private void UnlockPosition()
        {
            rigidbody.constraints = RigidbodyConstraints2D.None;
            isLocked = false;
        }

        private void ProcessCombo()
        {
            if (!neighbors.Any(IsSameColor)) return;
            
            onBeforeCombo?.Invoke();
            ScoreManager.Instance.IncrementCombo();
            Pop();
            onAfterCombo.Invoke();
        }

        private void HandleEdgeCaseDifferentColor()
        {
            if (!IsOutsideMainBubble()) return;

            MainBubble.Instance.OnGameEnd?.Invoke();

            Debug.Log("GAME OVER");
        }

        private bool IsOutsideMainBubble()
        {
            Vector3 center = SpawnManager.Instance.transform.position;
            float dist = (transform.position - center).magnitude;
            return dist + GetRadius() >= MainBubble.Instance.GetRadius();
        }

        public virtual void OnCollidedWithBubble(Bubble other)
        {
            bool sameColor = IsSameColor(other);

            Connect(other);
            if (firstCollision) onFirstCollision.Invoke();
            
            if (IsOutsideMainBubble())
            {
                // Pop will trigger a chain reaction on all the neighbors as needed
                if (sameColor) Pop();
                else // TODO: game over
                {
                    Invoke(nameof(HandleEdgeCaseDifferentColor), coyoteTime);
                    return;
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

        void OnCollisionEnter2D(Collision2D collision)
        {
            onCollision.Invoke();
            Bubble other = collision.collider.GetComponent<Bubble>();
            if (other != null) OnCollidedWithBubble(other);
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




