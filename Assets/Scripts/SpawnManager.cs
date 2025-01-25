using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject bubblePrefab;
    
    [SerializeField] private float secondsBetweenSpawns = 30.0f;
    
    [SerializeField] private int initialNumberOfSpawns = 5;
    
    [SerializeField] private int spawnLimit = 20;

    [SerializeField] private float initialForce = 5f;

    [SerializeField, Min(0f)] private float spawnRadius;

    [SerializeField] private List<Sprite> sprites = new List<Sprite>();

    private List<float> previousSpawnAngles = new List<float>();
    
    private Vector2 SpawnRange => GetRandomSpawnPosition();
    
    /// <summary>
    /// A pool for pre-instantiated bubbles
    /// </summary>
    private Queue<GameObject> _bubblePool;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
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
    void OnDrawGizmos()
    {
        Color previous = Handles.color;
        Handles.color = Color.yellow;
        Handles.DrawWireDisc(transform.position, transform.forward, spawnRadius);
        Handles.color = previous;
    }
#endif

    private void SetRandomBubbleColor(GameObject bubble)
    {
        bubble.GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, sprites.Count - 1)];
    }

    private void InitializeBubblePool()
    {
        _bubblePool = new();
        for (int i = 0; i < spawnLimit; i++)
        {
            GameObject bubble = Instantiate(bubblePrefab, transform);
            SetRandomBubbleColor(bubble);
            bubble.SetActive(false);
            _bubblePool.Enqueue(bubble);
        }
        Debug.Log($"Spawning {initialNumberOfSpawns} bubbles");
    }

    private void SpawnInitialBubbles()
    {
        for (int i = 0; i < initialNumberOfSpawns; i++)
        {
            var spawnPosition = GetRandomSpawnPosition();
            SpawnBubble(spawnPosition);
            Debug.Log($"Spawned a bubble at x: {spawnPosition.x}, y: {spawnPosition.y}");
        }
    }

    private IEnumerator SpawnBubbles()
    {
        while(_bubblePool.Count > 0)
        {
            yield return new WaitForSeconds(secondsBetweenSpawns);
            
            Vector3 spawnPosition = GetRandomSpawnPosition();
            SpawnBubble(spawnPosition);
            Debug.Log($"Spawned a bubble at x: {spawnPosition.x}, y: {spawnPosition.y}");
        }
    }

    private void SpawnBubble(Vector2 position)
    {
        if (_bubblePool.Count != 0)
        {
            GameObject bubble = _bubblePool.Dequeue();
            bubble.transform.position = position;
            Vector3 direction = (transform.position - bubble.transform.position).normalized; // destination - origin
            Rigidbody2D rigidbody = bubble.GetComponent<Rigidbody2D>();
            Debug.Log(direction * initialForce);
            bubble.SetActive(true);
            rigidbody.AddForce(direction * initialForce, ForceMode2D.Force);
        }
        
        Debug.LogWarning("No bubbles in the object pool.");
    }

    private Vector2 GetRandomSpawnPosition()
    {
        //Camera main = Camera.current;
        //float screenHeight = main.orthographicSize;
        //float screenWidth = main.aspect * screenHeight;

        float theta = Random.Range(0f, 2f * Mathf.PI);
        float randomX = spawnRadius * Mathf.Cos(theta);
        float randomY = spawnRadius * Mathf.Sin(theta);
        
        //var randomX = Random.Range(-screenWidth, screenWidth);
        //var randomY = Random.Range(-screenHeight, screenHeight);
        
        return new Vector2(randomX, randomY);
    }
    
    /// <summary>
    /// Get any nearby bubbles
    /// </summary>
    /// <returns>True if there are bubbles nearby, false if not</returns>
    private bool AreBubblesNearby(Vector2 position)
    {
        // TODO: get any nearby bubbles.
        return false;
    }
}
