using UnityEngine;
using UnityEngine.Serialization;

public class PooledProjectile : PoolableComponent
{
    [FormerlySerializedAs("projectile")] [SerializeField] private PlayerProjectile playerProjectile;

    public static PlayerProjectile GetProjectileFromPool(string poolName)
    {
        if (poolName.Length == 0) return null;
        PlayerProjectile playerProjectile = Services.PoolService?
            .Get<PooledProjectile>(poolName)
            ?.PlayerProjectile;
        return playerProjectile;

    }
    public PlayerProjectile PlayerProjectile => playerProjectile ??= GetComponent<PlayerProjectile>();
    
    public void ReturnToPool()
    {
        Services.PoolService?.Release(GetPoolName(), this);
    }
    
    public override string GetPoolName()
    {
        return playerProjectile.ProjectileId;
    }
}
