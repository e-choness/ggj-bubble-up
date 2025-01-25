using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


namespace Game
{
    /// <summary>
    /// The player controls this bubble
    /// </summary>
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

        public static MainBubble Instance;

        public static List<Bubble> bubbles = new();

        void Awake()
        {
            if (Instance != null) Destroy(gameObject);
            else Instance = this;
        }

        void FixedUpdate()
        {
            float speed = 0f;
            if (rotation.HasFlag(Rotation.Left)) speed += rotationSpeed;
            if (rotation.HasFlag(Rotation.Right)) speed -= rotationSpeed;
            bubbleContainer.transform.Rotate(transform.forward, speed * Time.fixedDeltaTime); 
        }

        public void RotateLeft(InputAction.CallbackContext context)
        {
            Debug.Log(context.started + " " + context.canceled);
            if (context.started) rotation |= Rotation.Left;
            else if (context.canceled) rotation &= ~Rotation.Left;
        }
        public void RotateRight(InputAction.CallbackContext context)
        {
            if (context.started) rotation |= Rotation.Right; 
            else if (context.canceled) rotation &= ~Rotation.Right;
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
            
        }
    }
}