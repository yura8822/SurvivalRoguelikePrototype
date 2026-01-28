using System;
using UnityEngine;

[RequireComponent(typeof(EnemyStats))]
[RequireComponent(typeof(EnemyMovement))]
[RequireComponent(typeof(PooledEnemy))]
public class EnemyController : MonoBehaviour, IGridElement
{
    [Header("Config")]
    [SerializeField] private string enemyId = "";
    
    private EnemyStats stats;
    private EnemyMovement movement; 
    private SpatialGridService grid;
    private PooledEnemy pooledEnemy;
    
    // IGridElement implementation
    public Transform Transform => transform;
    public Vector2Int CurrentCell { get; set; }

    public string EnemyId => enemyId;
    public PooledEnemy PooledEnemy => pooledEnemy ??= GetComponent<PooledEnemy>();
    

    private void Awake()
    {
        stats = GetComponent<EnemyStats>();
        movement = GetComponent<EnemyMovement>();
    }

    private void OnEnable()
    {
    }

    private void OnDestroy()
    {
        UnregisterFromGrid();
    }

    private void RegisterInGrid()
    {
        grid = Services.SpatialGridService;
        
        if (grid != null)
        {
            grid.AddElement(this);
        }
    }

    private void UnregisterFromGrid()
    {
        if (grid != null)
        {
            grid.RemoveElement(this);
        }
    }

    public void TakeDamage(float damage)
    {
        stats?.TakeDamage(damage);
        
        if (stats != null && !stats.IsAlive)
        {
            OnDeath();
        }
    }

    private void OnDeath()
    {
        Debug.Log("[EnemyController] Enemy died!");
        
        // TODO: Дроп опыта
        // SpawnExperience(stats.ExperienceDrop);

        UnregisterFromGrid();
        PooledEnemy.ReturnToPool();
     
    }
    
    public void InitializeEnemy()
    {
        if (Services.ConfigService == null || !Services.ConfigService.IsLoaded)
        {
            Debug.LogError("[EnemyController] ConfigService not available!");
            return;
        }

        EnemyConfig config = Services.ConfigService.GetEnemyConfig(enemyId);
        
        if (config == null)
        {
            Debug.LogError($"[EnemyController] Config for '{enemyId}' not found!");
            return;
        }

        stats?.Initialize(config);
        movement?.Initialize(stats, this);
        
        RegisterInGrid();
    }

    public EnemyStats GetStats() => stats;
}
