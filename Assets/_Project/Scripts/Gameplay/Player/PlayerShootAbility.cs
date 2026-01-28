using UnityEngine;

public class PlayerShootAbility : MonoBehaviour
{
    [Header("Ability Settings")]
    [SerializeField] private PlayerProjectile projectilePrefab;
    
    [Header("Spawn Settings")]
    [SerializeField] private Transform firePoint;

    private ShootAbilityConfig abilityConfig;
    private PlayerMovement playerMovement;
    private float attackCooldownTimer = 0f;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Start()
    {
        LoadAbilityConfig();
    }

    private void Update()
    {
        if (attackCooldownTimer > 0f)
        {
            attackCooldownTimer -= Time.deltaTime;
        }

        if (CanShoot() && playerMovement != null )
        {
            Vector2 direction = playerMovement.GetMoveDirection();
            Shoot(direction);
        }
    }
    
    private void LoadAbilityConfig()
    {
        if (Services.ConfigService == null || !Services.ConfigService.IsLoaded)
        {
            Debug.LogError("[PlayerShootAbility] ConfigService not available!");
            return;
        }

        abilityConfig = Services.ConfigService.GetShootingAbilityConfig(projectilePrefab.ProjectileId);

        if (abilityConfig == null)
        {
            Debug.LogError($"[PlayerShootAbility] Ability config '{projectilePrefab.ProjectileId}' not found!");
            return;
        }
    }

  
    public void Shoot(Vector2 direction)
    {
        if (abilityConfig == null)
        {
            Debug.LogWarning("[PlayerShootAbility] Ability config not loaded!");
            return;
        }

        if (direction.sqrMagnitude < 0.01f)
        {
            return; // 
        }

     
        PooledProjectile pooledProjectile = Services.PoolService.Get<PooledProjectile>(projectilePrefab.ProjectileId);

        if (pooledProjectile == null)
        {
            Debug.LogError($"[PlayerShootAbility] Failed to get projectile from pool '{projectilePrefab.ProjectileId}'!");
            return;
        }

 
        Vector3 spawnPosition = firePoint != null ? firePoint.position : transform.position;
        pooledProjectile.transform.position = spawnPosition;

   
        PlayerProjectile projectile = pooledProjectile.PlayerProjectile;
        
        if (projectile != null)
        {
            projectile.Initialize(
                direction,
                abilityConfig.projectileSpeed,
                abilityConfig.baseDamage
            );
        }
        attackCooldownTimer = abilityConfig.attackCooldown;
    }

 
    private bool CanShoot()
    {
        return abilityConfig != null && attackCooldownTimer <= 0f;
    }
}
