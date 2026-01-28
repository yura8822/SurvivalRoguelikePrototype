using UnityEngine;
using System.Collections.Generic;

public class PlayerAuraAbility : MonoBehaviour
{
    [Header("Ability Settings")]
    [SerializeField] private string abilityId = "aura";
    
    [Header("Visual Settings")]
    [SerializeField] private SpriteRenderer auraVisual;
    [SerializeField] private float visualScaleMultiplier = 1f; // Множитель для визуала
    
    [Header("Debug")]
    [SerializeField] private bool debugMode = false;
    [SerializeField] private bool showAuraRadius = true;
    
    private AuraAbilityConfig abilityConfig;
    private SpatialGridService grid;
    private Transform playerTransform;
    
    private float tickTimer = 0f;
    private bool isEnabled = true;

    private void Awake()
    {
        playerTransform = transform.parent;
        
        if (playerTransform == null)
        {
            Debug.LogError("[PlayerAuraAbility] This component must be child of Player!");
        }
    }

    private void Start()
    {
        LoadAbilityConfig();
        UpdateVisualScale();
    }

    private void Update()
    {
        if (!isEnabled || abilityConfig == null) return;

     
        tickTimer -= Time.deltaTime;

        if (tickTimer <= 0f)
        {
            DamageEnemiesInRadius();
            tickTimer = abilityConfig.attackCooldown;
        }
    }
    
    private void LoadAbilityConfig()
    {
        if (Services.ConfigService == null || !Services.ConfigService.IsLoaded)
        {
            Debug.LogError("[PlayerAuraAbility] ConfigService not available!");
            return;
        }

        abilityConfig = Services.ConfigService.GetAuraAbilityConfig(abilityId);

        if (abilityConfig == null)
        {
            Debug.LogError($"[PlayerAuraAbility] Aura ability config '{abilityId}' not found!");
            return;
        }

        grid = Services.SpatialGridService;

        if (debugMode)
        {
            Debug.Log($"[PlayerAuraAbility] Config loaded: {abilityId} - damage={abilityConfig.baseDamage}, tickRate={abilityConfig.attackCooldown}, radius={abilityConfig.radius}");
        }
    }

 
    private void UpdateVisualScale()
    {
        if (auraVisual == null || abilityConfig == null) return;
        
        float diameter = abilityConfig.radius * 2f * visualScaleMultiplier;
        auraVisual.transform.localScale = new Vector3(diameter, diameter, 1f);

        if (debugMode)
        {
            Debug.Log($"[PlayerAuraAbility] Visual scale updated: {diameter}");
        }
    }

 
    private void DamageEnemiesInRadius()
    {
        if (grid == null || playerTransform == null) return;

        Vector3 playerPosition = playerTransform.position;

     
        List<IGridElement> elementsInRadius = grid.GetElementsInRadius(playerPosition, abilityConfig.radius);

        int damagedCount = 0;

        foreach (var element in elementsInRadius)
        {
       
            if (element is EnemyController enemy && enemy.gameObject.activeInHierarchy)
            {
                enemy.TakeDamage(abilityConfig.baseDamage);
                damagedCount++;
            }
        }

        if (debugMode && damagedCount > 0)
        {
            Debug.Log($"[PlayerAuraAbility] Damaged {damagedCount} enemies for {abilityConfig.baseDamage} damage");
        }
    }
    
    public void SetEnabled(bool enabled)
    {
        isEnabled = enabled;
        
        if (auraVisual != null)
        {
            auraVisual.enabled = enabled; // Скрываем/показываем визуал
        }
        
        if (!enabled)
        {
            tickTimer = 0f; // Сбрасываем таймер
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!showAuraRadius || abilityConfig == null) return;

        Transform center = playerTransform != null ? playerTransform : transform.parent;
        if (center == null) return;

        // Рисуем радиус ауры
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        DrawCircle(center.position, abilityConfig.radius, 32);
    }

    private void DrawCircle(Vector3 center, float radius, int segments)
    {
        float angleStep = 360f / segments;
        Vector3 prevPoint = center + new Vector3(radius, 0, 0);

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 newPoint = center + new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }
    }
#endif
}
