using System.Collections.Generic;
using UnityEngine;

public class DefaultBalanceData
{
    public static GameBalanceData GetDefaultBalanceData()
    {
        return new GameBalanceData
        {
            player = new PlayerConfig
            {
                maxHealth = 100f,
                moveSpeed = 5f,
                collectionRadius = 10f
            },
            enemies = new List<EnemyConfig>
            {
                new EnemyConfig
                {
                    id = "fast_enemy",
                    health = 10f,
                    moveSpeed = 4f,
                    damage = 3f,
                    experienceDrop = 1
                },
                new EnemyConfig
                {
                    id = "slow_enemy",
                    health = 20f,
                    moveSpeed = 2f,
                    damage = 10f,
                    experienceDrop = 1
                }
            },
            shootAbilities = new List<ShootAbilityConfig>
            {
                new ShootAbilityConfig
                {
                    id = "direction_arrow",
                    baseDamage = 10f,
                    attackCooldown = 0.45f,
                    projectileSpeed = 17
                },
            },
            
            auraAbilities = new List<AuraAbilityConfig>
            {
                new AuraAbilityConfig
                {
                    id = "aura",
                    baseDamage = 1f,
                    attackCooldown = 0.2f,
                    radius = 3,
                },
            },
          
            experience = new ExperienceConfig
            {
                baseExpToLevel = 10,
                expScaling = 1.5f
            },
          
            spawn = new SpawnConfig 
            {
                spawnRadius = 40f,
                spawnInterval = 0.3f,
                maxEnemiesAlive = 30,
                enemySpawns = new List<EnemySpawnEntry>
                {
                    new EnemySpawnEntry
                    {
                        enemyId = "fast_enemy",
                        weight = 70  // 70% шанс
                    },
                    new EnemySpawnEntry
                    {
                        enemyId = "slow_enemy",
                        weight = 30  // 30% шанс
                    }
                }
            }
        };
      
    } 
}
