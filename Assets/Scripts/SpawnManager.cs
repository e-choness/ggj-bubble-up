using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject bubblePrefab;
    
    [SerializeField] private float secondsBetweenSpawns = 30.0f;
    
    [SerializeField] private int initialNumberOfSpawns = 5;
    
    [SerializeField] private int spawnLimit = 20;
    
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

    private void InitializeBubblePool()
    {
        _bubblePool = new();
        for (var i = 0; i < spawnLimit; i++)
        {
            var bubble = Instantiate(bubblePrefab, transform);
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
            
            var spawnPosition = GetRandomSpawnPosition();
            SpawnBubble(spawnPosition);
            Debug.Log($"Spawned a bubble at x: {spawnPosition.x}, y: {spawnPosition.y}");
        }
    }

    private void SpawnBubble(Vector2 position)
    {
        if (_bubblePool.Count != 0)
        {
            var bubble = _bubblePool.Dequeue();
            bubble.transform.position = position;
            bubble.SetActive(true);
        }
        
        Debug.LogWarning("No bubbles in the object pool.");
    }

    private Vector2 GetRandomSpawnPosition()
    {
        var main = Camera.current;
        var screenHeight = main.orthographicSize;
        var screenWidth = main.aspect * main.orthographicSize;
        
        var randomX = Random.Range(-screenWidth, screenWidth);
        var randomY = Random.Range(-screenHeight, screenHeight);
        
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
