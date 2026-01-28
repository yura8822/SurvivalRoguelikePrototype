using System;
using UnityEngine;
using UnityEngine.Events;

public class PlayerStats : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Collection")]
    [SerializeField] private float collectionRadius = 2f;

  
    public UnityAction<float, float> OnHealthChanged; 
    public UnityAction OnDeath;

    
    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    public float MoveSpeed => moveSpeed;
    public float CollectionRadius => collectionRadius;
    public bool IsAlive => currentHealth > 0f;


    public void Initialize(PlayerConfig config)
    {
        if (config == null)
        {
            Debug.LogWarning("[PlayerStats] Config is null, using default values!");
            currentHealth = maxHealth;
            return;
        }

        maxHealth = config.maxHealth;
        moveSpeed = config.moveSpeed;
        collectionRadius = config.collectionRadius;

        // Полное здоровье при инициализации
        currentHealth = maxHealth;

        Debug.Log($"[PlayerStats] Initialized: HP={maxHealth}, Speed={moveSpeed}, Radius={collectionRadius}");
    }

  
    public void TakeDamage(float damage)
    {
        if (!IsAlive) return;

        currentHealth = Mathf.Max(0f, currentHealth - damage);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        Debug.Log($"[PlayerStats] Took {damage} damage. HP: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

   
    public void Heal(float amount)
    {
        if (!IsAlive) return;

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        Debug.Log($"[PlayerStats] Healed {amount}. HP: {currentHealth}/{maxHealth}");
    }

 
    public void IncreaseMaxHealth(float amount)
    {
        maxHealth += amount;
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth); // Добавляем текущее тоже
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        Debug.Log($"[PlayerStats] Max health increased by {amount}. New max: {maxHealth}");
    }

   
    public void SetMoveSpeed(float speed)
    {
        moveSpeed = Mathf.Max(0f, speed);
    }


    public void ModifyMoveSpeed(float multiplier)
    {
        moveSpeed *= multiplier;
    }


    public void SetCollectionRadius(float radius)
    {
        collectionRadius = Mathf.Max(0f, radius);
    }


    private void Die()
    {
        Debug.Log("[PlayerStats] Player died!");
        OnDeath?.Invoke();
    }


    public void Reset(PlayerConfig config)
    {
        Initialize(config);
    }


}
