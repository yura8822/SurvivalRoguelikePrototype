using UnityEngine;

public class PooledEnemy : PoolableComponent
{
    [SerializeField] private EnemyController enemyController;

    public static EnemyController GetEnemyControllerFromPool(string poolName)
    {
        if (poolName.Length == 0) return null;
        EnemyController enemyController = Services.PoolService?
            .Get<PooledEnemy>(poolName)
            ?.EnemyController;
        enemyController?.InitializeEnemy();
        return enemyController;
    }
    
    public EnemyController EnemyController => enemyController ??= GetComponent<EnemyController>();
    
    public void ReturnToPool()
    {
        Services.PoolService?.Release(GetPoolName(), this);
    }
    
    public override string GetPoolName()
    {
        return enemyController.EnemyId;
    }
}
