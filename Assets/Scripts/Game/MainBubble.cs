using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game
{
    /// <summary>
    /// The player controls this bubble
    /// </summary>
    [RequireComponent(typeof(CircleCollider2D))]
    public class MainBubble : MonoBehaviour
    {
        /// <summary>
        /// Degrees per second
        /// </summary>
        [Tooltip("Degrees per second")]
        [SerializeField, Min(0f)] private float rotationSpeed = 5f;

        [SerializeField] private GameObject bubbleContainer;

        public float velocityRadiusFactor = 2f;

        [SerializeField] private GameObject bubblePrefab;

        [Header("Audio")]
        [SerializeField] private AudioSource chainPopAudio;

        private Rotation rotation;

        [System.Flags, System.Serializable]
        private enum Rotation
        {
            Nothing = 0,
            Left = 1 << 0,
            Right = 1 << 1,
        }

        private CircleCollider2D _collider;
        public new CircleCollider2D collider 
        {
            get
            {
                if (_collider == null) _collider = GetComponent<CircleCollider2D>();
                return _collider;
            }
        }

        public static MainBubble Instance;

        public static List<Bubble> bubbles = new();
        private AudioSource _audioSource;

        public static Bubble centralBubble;

        public List<Bubble> bubblesPoppedThisFrame = new();

        public Action OnGameEnd;

        void Awake()
        {
            if (Instance != null) Destroy(gameObject);
            else 
            {
                Instance = this;
                _audioSource = GetComponent<AudioSource>();
            }
        }

        void Start()
        {
            CreateStartingBubbles();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow)) StartRotateLeft();
            else if (Input.GetKeyUp(KeyCode.LeftArrow)) StopRotateLeft();
            else if (Input.GetKeyDown(KeyCode.RightArrow)) StartRotateRight();
            else if (Input.GetKeyUp(KeyCode.RightArrow)) StopRotateRight();
        }

        void LateUpdate()
        {
            ProcessChainPop();
            bubblesPoppedThisFrame.Clear();
        }

        void FixedUpdate()
        {
            float speed = 0f;
            if (rotation.HasFlag(Rotation.Left)) speed += rotationSpeed;
            if (rotation.HasFlag(Rotation.Right)) speed -= rotationSpeed;
            bubbleContainer.transform.Rotate(transform.forward, speed * Time.fixedDeltaTime); 
        }

        private void ProcessChainPop()
        {
            if (bubblesPoppedThisFrame.Count > 2) chainPopAudio.Play();
        }

        public float GetRadius() => (transform.TransformPoint(new Vector2(collider.radius, 0f)) - transform.position).magnitude;

        public void StartRotateLeft()
        {
            rotation |= Rotation.Left;
        }
        public void StopRotateLeft()
        {
            rotation &= ~Rotation.Left;
        }
        public void StartRotateRight()
        {
            rotation |= Rotation.Right;
        }
        public void StopRotateRight()
        {
            rotation &= ~Rotation.Right;
        }

        public static void AddBubble(Bubble bubble)
        {
            if (bubbles.Contains(bubble)) throw new System.Exception("Cannot add a bubble that has already been added before");
            bubble.transform.SetParent(Instance.bubbleContainer.transform, true);
            bubbles.Add(bubble);
        }

        public static void RemoveBubble(Bubble bubble)
        {
            if (!bubbles.Contains(bubble)) throw new System.Exception("Cannot remove bubble because it was never added");
            bubbles.Remove(bubble);
        }

        public void Pop()
        {
            _audioSource.Play();
        }

        private void CreateStartingBubbles()
        {
            GameObject central = null;
            central = Instantiate(bubblePrefab, transform.position, Quaternion.identity, null);

            Bubble bubble = central.GetComponent<Bubble>();
            List<Color> colors = new List<Color>(bubble.colors);

            Queue<Color> colorQueue = new Queue<Color>();

            void SetColor(Bubble bubble)
            {
                if (colorQueue.Count == 0) // refill the queue
                {
                    foreach (Color c in colors.Shuffle()) colorQueue.Enqueue(c);
                }
                bubble.SetColor(colorQueue.Dequeue());
            }

            float radius = bubble.GetRadius();

            SetColor(bubble);
            AddBubble(bubble);
            
            float theta = 0f;
            Vector3 position = Vector3.zero;
            GameObject go = null;
            for (int i = 0; i < 6; i ++)
            {
                position.x = 2.1f * radius * Mathf.Cos(theta);
                position.y = 2.1f * radius * Mathf.Sin(theta);
                go = Instantiate(bubblePrefab, transform.position + position, Quaternion.identity, null);
                Bubble b = go.GetComponent<Bubble>();
                SetColor(b);
                AddBubble(b);
                theta += 60f * Mathf.Deg2Rad;
            }

            foreach (Bubble b in bubbles) b.firstCollision = false;
        }

        #region Editor
        #if UNITY_EDITOR
        void OnDrawGizmos()
        {
            Color previous = Handles.color;
            Handles.color = Color.cyan;
            Handles.DrawWireDisc(transform.position, transform.forward, GetRadius());
            Handles.color = previous;
        }
        #endif
        #endregion
    }
}