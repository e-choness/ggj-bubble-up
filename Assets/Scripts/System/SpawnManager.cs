using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEditor;
using UnityEngine;

namespace System
{
    public class SpawnManager : Singleton<SpawnManager>
    {
        /// <summary>
        /// Bubble prefab to for spawning
        /// </summary>
        [SerializeField] private GameObject bubblePrefab;
    
        /// <summary>
        /// Seconds between bubble spawns
        /// </summary>
        [SerializeField] private float secondsBetweenSpawns = 30.0f;
    
        /// <summary>
        /// The initial number of bubbles to spawn
        /// </summary>
        [SerializeField] private int initialNumberOfSpawns = 5;
    
        /// <summary>
        /// The maximum number of bubbles to spawn
        /// Also is the object pool limit
        /// </summary>
        [SerializeField] private int spawnLimit = 20;

        /// <summary>
        /// The range where the bubble spawns on the screen
        /// </summary>
        [SerializeField, Min(0f)] private float spawnRadius;
        [SerializeField] private List<GameObject> specialBubblePrefabs = new();
        [SerializeField] private List<float> specialBubbleSpawnChances = new();
    

        private List<float> previousSpawnAngles = new();
    
        /// <summary>
        /// A pool for pre-instantiated bubbles
        /// </summary>
        private Queue<GameObject> _bubblePool;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public override void Awake()
        {
            base.Awake();
            InitializeBubblePool();
        }

        private void Start()
        {
            // Spawn 5 bubbles at start
            SpawnInitialBubbles();
        
            // Spawn the rest of the bubbles in the pool every 5 seconds
            StartCoroutine(SpawnBubbles());
        }

#if UNITY_EDITOR
        /// <summary>
        /// For debugging purposes
        /// </summary>
        private void OnDrawGizmos()
        {
            Color previous = Handles.color;
            Handles.color = Color.yellow;
            Handles.DrawWireDisc(transform.position, transform.forward, spawnRadius);
            Handles.color = previous;
        }
#endif

        private void InitializeBubblePool()
        {
            _bubblePool = new Queue<GameObject>();
            for (var i = 0; i < spawnLimit; i++)
            {
                var bubble = Instantiate(GetRandomBubble(), transform);
                bubble.GetComponent<Bubble>().SetRandomColor();
                bubble.SetActive(false);
                _bubblePool.Enqueue(bubble);
            }
            //Debug.Log($"Spawning {initialNumberOfSpawns} bubbles");
        }

        private void SpawnInitialBubbles()
        {
            for (var i = 0; i < initialNumberOfSpawns; i++)
            {
                var spawnPosition = GetRandomSpawnPosition();
                SpawnBubble(spawnPosition);
                //Debug.Log($"Spawned a bubble at x: {spawnPosition.x}, y: {spawnPosition.y}");
            }
        }

        private IEnumerator SpawnBubbles()
        {
            while(_bubblePool.Count > 0)
            {
                yield return new WaitForSeconds(secondsBetweenSpawns);
            
                Vector3 spawnPosition = GetRandomSpawnPosition();
                SpawnBubble(spawnPosition);
                //Debug.Log($"Spawned a bubble at x: {spawnPosition.x}, y: {spawnPosition.y}");
            }
        }

        private void SpawnBubble(Vector2 position)
        {
            if (_bubblePool.Count != 0)
            {
                GameObject bubble = _bubblePool.Dequeue();
                bubble.transform.position = position;
                bubble.SetActive(true);
                return;
            }

            Debug.LogWarning("No bubbles in the object pool.");
        }

        private Vector2 GetRandomSpawnPosition()
        {
            var theta = UnityEngine.Random.Range(0f, 2f * Mathf.PI);
            var randomX = spawnRadius * Mathf.Cos(theta);
            var randomY = spawnRadius * Mathf.Sin(theta);
            return new Vector2(randomX, randomY);
        }

        private GameObject GetRandomBubble()
        {
            float a = UnityEngine.Random.Range(0f, 1f);
            for (int i =0; i < specialBubblePrefabs.Count; i ++)
            {
                if (a < specialBubbleSpawnChances[i]) return specialBubblePrefabs[i];
            }
            //foreach ( special in specialBubbles.Shuffle())
            //{
             //   if (a < special.spawnChance) return special.prefab;
            //}
            return bubblePrefab;
        }

        public void ReturnToPool(Bubble bubble)
        {
            bubble.gameObject.SetActive(false);
            bubble.transform.SetParent(transform);
            bubble.transform.position = new Vector3(1000f, 1000f, transform.position.z);
            _bubblePool.Enqueue(bubble.gameObject);
        }
    
        /// <summary>
        /// Get any nearby bubbles
        /// TODO: Might not need it because the design changed where the bubbles can collide
        /// </summary>
        /// <returns>True if there are bubbles nearby, false if not</returns>
        private bool AreBubblesNearby(Vector2 position)
        {
            // TODO: get any nearby bubbles.
            return false;
        }
    }
}
