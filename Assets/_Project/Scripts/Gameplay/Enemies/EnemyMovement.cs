using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private EnemyStats stats;
    private EnemyController controller;
    private Transform target;
    private SpatialGridService grid;
    private bool isEnabled = true;

    public void Initialize(EnemyStats enemyStats, EnemyController enemyController)
    {
        stats = enemyStats;
        controller = enemyController;
        grid = Services.SpatialGridService;
        
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!isEnabled || stats == null || !stats.IsAlive) return;

        UpdateTarget();
        
        if (target != null)
        {
            MoveTowardsTarget();
            UpdateVisuals();
        }
    }

    private void UpdateTarget()
    {
        if (target == null && Services.PlayerService.HasPlayer)
        {
            target = Services.PlayerService.GetPlayerTransform();
        }
    }

    private void MoveTowardsTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Vector3 newPosition = transform.position + direction * stats.MoveSpeed * Time.deltaTime;

        if (grid != null)
        {
            Vector2Int nextCell = grid.GetCellCoords(newPosition);
            
            if (!grid.IsCellOccupied(nextCell, controller))
            {
                UpdatePosition(newPosition);
            }
        }
        else
        {
            transform.position = newPosition;
        }
    }

    private void UpdatePosition(Vector3 newPosition)
    {
        if (controller == null || grid == null) return;

        Vector2Int oldCell = controller.CurrentCell;
        Vector2Int newCell = grid.GetCellCoords(newPosition);

        transform.position = newPosition;

        if (oldCell != newCell)
        {
            grid.RemoveElement(controller);
            grid.AddElement(controller);
        }
    }

    private void UpdateVisuals()
    {
        if (spriteRenderer == null || target == null) return;
        float direction = target.position.x - transform.position.x;
        if (direction < -0.01f)
        {
            spriteRenderer.flipX = true;
        }
        else if (direction > 0.01f)
        {
            spriteRenderer.flipX = false;
        }
    }

    public void SetEnabled(bool enabled)
    {
        isEnabled = enabled;
    }
}
