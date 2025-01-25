using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

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

        void Awake()
        {
            if (Instance != null) Destroy(gameObject);
            else 
            {
                Instance = this;
                _audioSource = GetComponent<AudioSource>();
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow)) StartRotateLeft();
            else if (Input.GetKeyUp(KeyCode.LeftArrow)) StopRotateLeft();
            else if (Input.GetKeyDown(KeyCode.RightArrow)) StartRotateRight();
            else if (Input.GetKeyUp(KeyCode.RightArrow)) StopRotateRight();
        }

        void FixedUpdate()
        {
            float speed = 0f;
            if (rotation.HasFlag(Rotation.Left)) speed += rotationSpeed;
            if (rotation.HasFlag(Rotation.Right)) speed -= rotationSpeed;
            bubbleContainer.transform.Rotate(transform.forward, speed * Time.fixedDeltaTime); 
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