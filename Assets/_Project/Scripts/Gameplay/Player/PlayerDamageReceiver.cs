using UnityEngine;

using UnityEngine;
using System.Collections.Generic;

public class PlayerDamageReceiver : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private float damageRadius = 1f;
    [SerializeField] private float damageInterval = 1f; 
    
    [Header("Debug")]
    [SerializeField] private bool debugMode = true;
    [SerializeField] private bool showDamageRadius = true;
    
    private PlayerStats playerStats;
    private SpatialGridService grid;
    private float damageTimer = 0f;
    private bool isEnabled = true;

    private void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
    }

    private void Start()
    {
        grid = Services.SpatialGridService;
        
        if (grid == null)
        {
            Debug.LogError("[PlayerDamageReceiver] SpatialGridService not found!");
        }
        
        if (playerStats == null)
        {
            Debug.LogError("[PlayerDamageReceiver] PlayerStats not found!");
        }
    }

    private void Update()
    {
        if (!isEnabled || grid == null || playerStats == null) return;
        
        if (!playerStats.IsAlive) return;

      
        damageTimer -= Time.deltaTime;

        if (damageTimer <= 0f)
        {
            CheckForEnemiesAndTakeDamage();
            damageTimer = damageInterval; // Сбрасываем таймер
        }
    }


    private void CheckForEnemiesAndTakeDamage()
    {
        // Получаем всех врагов в радиусе через Grid
        List<IGridElement> elementsInRadius = grid.GetElementsInRadius(transform.position, damageRadius);

        if (elementsInRadius.Count == 0) return;

        float totalDamage = 0f;
        int enemyCount = 0;

        foreach (var element in elementsInRadius)
        {
            // Проверяем что это враг
            if (element is EnemyController enemy && enemy.gameObject.activeInHierarchy)
            {
                EnemyStats enemyStats = enemy.GetStats();
                
                if (enemyStats != null && enemyStats.IsAlive)
                {
                    totalDamage += enemyStats.Damage;
                    enemyCount++;
                }
            }
        }

       
        if (totalDamage > 0f)
        {
            playerStats.TakeDamage(totalDamage);

            if (debugMode)
            {
                Debug.Log($"[PlayerDamageReceiver] Took {totalDamage} damage from {enemyCount} enemies");
            }
        }
    }

  
    public void SetEnabled(bool enabled)
    {
        isEnabled = enabled;
        
        if (!enabled)
        {
            damageTimer = 0f; // Сбрасываем таймер
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!showDamageRadius) return;

        // Рисуем радиус проверки врагов
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        DrawCircle(transform.position, damageRadius, 32);
    }

    private void DrawCircle(Vector3 center, float radius, int segments)
    {
        float angleStep = 360f / segments;
        Vector3 prevPoint = center + new Vector3(radius, 0, 0);

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 newPoint = center + new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }
    }
#endif
}

