using UnityEngine;

[RequireComponent((typeof(PooledProjectile)))]
public class PlayerProjectile : MonoBehaviour
{
    [Header("Config")] 
    [SerializeField] private string projectileId;
    public string ProjectileId => projectileId;
    
    private PooledProjectile pooledProjectile;
    public PooledProjectile PooledProjectile => pooledProjectile ??= GetComponent<PooledProjectile>();


    [Header("Settings")]
    [SerializeField] private float speed = 15f;
    [SerializeField] private float damage = 10f;


    private Vector3 direction;
    private SpatialGridService grid;
    private Vector2Int lastCheckedCell = new Vector2Int(-9999, -9999);

 

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
        CheckForHit();
    }

    public void Initialize(Vector3 shootDirection, float projectileSpeed, float projectileDamage)
    {
        direction = shootDirection.normalized;
        speed = projectileSpeed;
        damage = projectileDamage;
        
        grid = Services.SpatialGridService;
        lastCheckedCell = new Vector2Int(-9999, -9999);
        
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

  
    private void CheckForHit()
    {
        if (grid == null) return;

        Vector2Int currentCell = grid.GetCellCoords(transform.position);

        if (currentCell == lastCheckedCell)
        {
            return;
        }

        lastCheckedCell = currentCell;

       
        if (!grid.IsCellOccupied(currentCell))
        {
            return; 
        }

        var elementsInCell = grid.GetElementsInCell(currentCell);


        foreach (var element in elementsInCell)
        {
            if (element is EnemyController enemy && enemy.gameObject.activeInHierarchy)
            {
                OnHitEnemy(enemy);
                return; 
            }
        }
    }

    private void OnHitEnemy(EnemyController enemy)
    {
        if (enemy == null || !enemy.gameObject.activeInHierarchy)
        {
            return;
        }

        enemy.TakeDamage(damage);
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        PooledProjectile?.ReturnToPool();
    }

}
