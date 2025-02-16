using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemyType
    {
        public string prefabTag;
        public float spawnWeight;
        public int minWaveRequired;
    }

    [SerializeField] private List<EnemyType> enemyTypes;
    [SerializeField] private float baseSpawnInterval = 2f;
    [SerializeField] private float spawnIntervalDecreaseRate = 0.1f;
    [SerializeField] private float minSpawnInterval = 0.5f;
    [SerializeField] private float spawnRadius = 15f;

    private float currentSpawnInterval;
    private Transform playerTransform;
    private bool isSpawning;

    private void Start()
    {
        currentSpawnInterval = baseSpawnInterval;
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        
        GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
        StartSpawning();
    }

    private void HandleGameStateChanged(GameManager.GameState newState)
    {
        if (newState == GameManager.GameState.Playing)
        {
            StartSpawning();
        }
        else
        {
            StopSpawning();
        }
    }

    private void StartSpawning()
    {
        if (!isSpawning)
        {
            isSpawning = true;
            StartCoroutine(SpawnRoutine());
        }
    }

    private void StopSpawning()
    {
        isSpawning = false;
        StopAllCoroutines();
    }

    private IEnumerator SpawnRoutine()
    {
        while (isSpawning)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(currentSpawnInterval);
            
            // Decrease spawn interval based on current wave
            float difficultyMultiplier = GameManager.Instance.GetCurrentDifficulty();
            currentSpawnInterval = Mathf.Max(
                minSpawnInterval,
                baseSpawnInterval - (spawnIntervalDecreaseRate * GameManager.Instance.CurrentWave)
            );
        }
    }

    private void SpawnEnemy()
    {
        EnemyType enemyType = SelectEnemyType();
        if (enemyType == null) return;

        Vector3 spawnPosition = GetSpawnPosition();
        GameObject enemy = PoolManager.Instance.SpawnFromPool(
            enemyType.prefabTag,
            spawnPosition,
            Quaternion.identity
        );

        if (enemy != null)
        {
            EnemyBehavior behavior = enemy.GetComponent<EnemyBehavior>();
            if (behavior != null)
            {
                behavior.Initialize(playerTransform);
            }
        }
    }

    private EnemyType SelectEnemyType()
    {
        // Filter available enemy types based on current wave
        List<EnemyType> availableTypes = enemyTypes.FindAll(
            e => e.minWaveRequired <= GameManager.Instance.CurrentWave
        );

        if (availableTypes.Count == 0) return null;

        // Calculate total weight
        float totalWeight = 0f;
        foreach (var type in availableTypes)
        {
            totalWeight += type.spawnWeight;
        }

        // Random selection based on weights
        float random = Random.Range(0f, totalWeight);
        float currentWeight = 0f;

        foreach (var type in availableTypes)
        {
            currentWeight += type.spawnWeight;
            if (random <= currentWeight)
            {
                return type;
            }
        }

        return availableTypes[0];
    }

    private Vector3 GetSpawnPosition()
    {
        float angle = Random.Range(0f, 360f);
        Vector3 direction = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;
        Vector3 spawnPosition = playerTransform.position + direction * spawnRadius;
        
        // Ensure spawn position is on the ground
        spawnPosition.y = 0f;
        
        return spawnPosition;
    }

    private void OnDrawGizmosSelected()
    {
        // Draw spawn radius in editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
        }
    }
}