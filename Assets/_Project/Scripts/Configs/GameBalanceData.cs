using System;
using System.Collections.Generic;


[Serializable]
public class GameBalanceData
{
    public PlayerConfig player;
    public List<EnemyConfig> enemies;
    public List<ShootAbilityConfig> shootAbilities;
    public List<AuraAbilityConfig> auraAbilities;
    public ExperienceConfig experience;
    public SpawnConfig spawn;
}

[Serializable]
public class SpawnConfig
{
    public float spawnRadius;        
    public float spawnInterval;          
    public int maxEnemiesAlive;
    public List<EnemySpawnEntry> enemySpawns;
}
[Serializable]
public class EnemySpawnEntry
{
    public string enemyId;                       
    public int weight = 1;                       
}


[Serializable]
public class PlayerConfig
{
    public float maxHealth;
    public float moveSpeed;
    public float collectionRadius;
}


[Serializable]
public class EnemyConfig
{
    public string id;              
    public float health;
    public float moveSpeed;
    public float damage;
    public int experienceDrop;
}


[Serializable]
public class ShootAbilityConfig
{
    public string id;
    public float baseDamage;
    public float attackCooldown;
    public float projectileSpeed;
}

[Serializable]
public class AuraAbilityConfig
{
    public string id;
    public float baseDamage;
    public float attackCooldown;
    public float radius;
}

[Serializable]
public class ExperienceConfig
{
    public int baseExpToLevel;     
    public float expScaling;
}