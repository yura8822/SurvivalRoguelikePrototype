using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private bool debugMode = false;
    [SerializeField] private bool showSpawnRadius = true;

    private SpawnConfig config;
    private List<EnemyController> aliveEnemies = new List<EnemyController>();
    private float spawnTimer = 0f;
    private bool isSpawning = false;

    // Weighted spawn system
    private List<string> weightedEnemyPool = new List<string>();

    private void Update()
    {
        if (!isSpawning) return;

        CleanupDeadEnemies();

        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f && CanSpawn())
        {
            SpawnRandomEnemy();
            spawnTimer = config.spawnInterval;
        }
    }

    private void LoadConfig()
    {
        if (Services.ConfigService == null || !Services.ConfigService.IsLoaded)
        {
            Debug.LogError("[EnemySpawner] ConfigService not available!");
            return;
        }

        config = Services.ConfigService.GetSpawnConfig();

        if (config == null)
        {
            Debug.LogError("[EnemySpawner] SpawnConfig not found!");
            return;
        }

        Debug.Log($"[EnemySpawner] Config loaded: radius={config.spawnRadius}, interval={config.spawnInterval}");
    }


    private void BuildWeightedPool()
    {
        if (config == null || config.enemySpawns == null) return;

        weightedEnemyPool.Clear();

        foreach (var entry in config.enemySpawns)
        {
            for (int i = 0; i < entry.weight; i++)
            {
                weightedEnemyPool.Add(entry.enemyId);
            }
        }

        Debug.Log($"[EnemySpawner] Weighted pool built: {weightedEnemyPool.Count} entries");
    }

  
    private void SpawnRandomEnemy()
    {
        if (weightedEnemyPool.Count == 0)
        {
            Debug.LogWarning("[EnemySpawner] Weighted pool is empty!");
            return;
        }

        string enemyId = weightedEnemyPool[Random.Range(0, weightedEnemyPool.Count)];

        Vector3 spawnPosition = GetRandomSpawnPosition();

       
        if (!IsCellFree(spawnPosition))
        {
            if (debugMode)
            {
                Debug.Log("[EnemySpawner] Spawn position is occupied, skipping this spawn");
            }
            return;
        }

        EnemyController enemy = SpawnEnemy(enemyId, spawnPosition);

        if (enemy != null)
        {
            aliveEnemies.Add(enemy);

            if (debugMode)
            {
                Debug.Log($"[EnemySpawner] Spawned {enemyId} at {spawnPosition}. Alive: {aliveEnemies.Count}/{config.maxEnemiesAlive}");
            }
        }
    }
    
    private bool IsCellFree(Vector3 position)
    {
        var grid = Services.SpatialGridService;
        if (grid == null) return true;
    
        Vector2Int cell = grid.GetCellCoords(position);
        return !grid.IsCellOccupied(cell);
    }


    private EnemyController SpawnEnemy(string enemyId, Vector3 position)
    {
        EnemyController controller = PooledEnemy.GetEnemyControllerFromPool(enemyId);

        if (controller == null)
        {
            Debug.LogError("[EnemySpawner] Failed to get enemy from pool!");
            return null;
        }

        controller.transform.position = position;

        return controller;
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector3 center = Vector3.zero;

        if (Services.PlayerService.HasPlayer)
        {
            center = Services.PlayerService.GetPlayerPosition();
        }

        // Случайный угол в радианах
        float angle = Random.Range(0f, Mathf.PI * 2f);

        // Точка на окружности
        Vector3 offset = new Vector3(
            Mathf.Cos(angle) * config.spawnRadius,
            Mathf.Sin(angle) * config.spawnRadius,
            0f
        );

        return center + offset;
    }


    private bool CanSpawn()
    {
        if (config == null) return false;
        return aliveEnemies.Count < config.maxEnemiesAlive;
    }

    /// <summary>
    /// Очищаем список от мертвых/уничтоженных врагов
    /// </summary>
    private void CleanupDeadEnemies()
    {
        aliveEnemies.RemoveAll(enemy => enemy == null || !enemy.gameObject.activeInHierarchy);
    }
    
    public void Initialize()
    {
        LoadConfig();
        BuildWeightedPool();
    }
    

    /// <summary>
    /// Запустить спавн
    /// </summary>
    public void StartSpawning()
    {
        isSpawning = true;
        spawnTimer = config != null ? config.spawnInterval : 2f;
        Debug.Log("[EnemySpawner] Spawning started");
    }

    /// <summary>
    /// Остановить спавн
    /// </summary>
    public void StopSpawning()
    {
        isSpawning = false;
        Debug.Log("[EnemySpawner] Spawning stopped");
    }


    /// <summary>
    /// Очистить всех живых врагов
    /// </summary>
    public void ClearAllEnemies()
    {
        foreach (var enemy in aliveEnemies)
        {
            if (enemy != null && enemy.gameObject.activeInHierarchy)
            {
                enemy.PooledEnemy?.ReturnToPool();
            }
        }

        aliveEnemies.Clear();
        Debug.Log("[EnemySpawner] All enemies cleared");
    }

    public int GetAliveCount() => aliveEnemies.Count;
    public int GetMaxCount() => config != null ? config.maxEnemiesAlive : 0;


    

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!showSpawnRadius) return;

        Vector3 center = Vector3.zero;

        if (Application.isPlaying && Services.PlayerService.HasPlayer)
        {
            center = Services.PlayerService.GetPlayerPosition();
        }

        float radius = config != null ? config.spawnRadius : 10f;

        // Рисуем окружность спавна
        Gizmos.color = Color.red;
        DrawCircle(center, radius, 32);
    }

    private void DrawCircle(Vector3 center, float radius, int segments)
    {
        float angleStep = 360f / segments;
        Vector3 prevPoint = center + new Vector3(radius, 0, 0);

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 newPoint = center + new Vector3(
                Mathf.Cos(angle) * radius,
                Mathf.Sin(angle) * radius,
                0
            );

            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }
    }
#endif
}