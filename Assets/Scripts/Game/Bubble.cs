using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;


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

        public UnityEvent onCollision = new();
        public UnityEvent onCollisionWithSameColor = new();
        public UnityEvent onCollisionWithDifferentColor = new();
        public UnityEvent onCollisionOutsideMainBubble = new();
        public UnityEvent onReachedCenter = new();
        public UnityEvent onPop = new();
        public UnityEvent onBeforeCombo = new();
        public UnityEvent onAfterCombo = new();
        
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

        [HideInInspector] public List<Bubble> neighbors = new();

        [HideInInspector] public bool firstCollision = true;

        private Sprite originalSprite;

        //potentially use the same value for all the vector values

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _rigidbody = GetComponent<Rigidbody2D>();
            _audioSource = GetComponent<AudioSource>();
            _animator = GetComponent<Animator>();
            originalSprite = _spriteRenderer.sprite;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (!isLocked) SetVelocity();
            if (IsAtCenter()) OnReachCenter();
        }

        void Reset()
        {
            _spriteRenderer.sprite = originalSprite;
            _animator.SetBool("isPopped", false);
            _audioSource.Stop();
            if (isLocked) UnlockPosition();
            neighbors.Clear();
            firstCollision = true;
        }
    
        public void Pop()
        {
            _animator.SetBool("isPopped", true);
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

        public void SetRandomColor() => SetColor(colors[Random.Range(0, colors.Count - 1)]);

        public void SetColor(Color color) 
        {
            _spriteRenderer.color = color;
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
            _rigidbody.linearVelocity = direction * GetVelocity();
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
            _rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
            isLocked = true;
        }
        private void UnlockPosition()
        {
            _rigidbody.constraints = RigidbodyConstraints2D.None;
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
            if (firstCollision) _audioSource.Play();
            
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




