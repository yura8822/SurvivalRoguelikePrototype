using UnityEngine;
using UnityEngine.Events;

public class EnemyStats : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float maxHealth = 10f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float damage = 5f;
    [SerializeField] private int experienceDrop = 1;
    
    private float currentHealth;
    
    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    public float MoveSpeed => moveSpeed;
    public float Damage => damage;
    public int ExperienceDrop => experienceDrop;
    public bool IsAlive => currentHealth > 0f;
    
    public UnityAction OnDeath;


 
    public void Initialize(EnemyConfig config)
    {
        if (config == null)
        {
            Debug.LogWarning("[EnemyStats] Config is null!");
            currentHealth = maxHealth;
            return;
        }

        maxHealth = config.health;
        moveSpeed = config.moveSpeed;
        damage = config.damage;
        experienceDrop = config.experienceDrop;
        
        currentHealth = maxHealth;
    }

   
    public void TakeDamage(float damage)
    {
        if (!IsAlive) return;

        currentHealth = Mathf.Max(0f, currentHealth - damage);
        
        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        OnDeath?.Invoke();
    }

  
}
