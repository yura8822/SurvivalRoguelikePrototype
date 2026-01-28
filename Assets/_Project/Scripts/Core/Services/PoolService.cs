using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

public class PoolService : MonoBehaviour
{
    [Header("Pool Configurations")] 
    [SerializeField] private PoolConfig[] poolConfigs;

    private Dictionary<string, IObjectPool<PoolableComponent>> pools = new();
    private Dictionary<string, PoolConfig> configs = new();
    private Transform poolParent;

    
    public void InitializePools()
    {
        // Создаем родительский объект для пулов
        poolParent = new GameObject("Pool Container").transform;
        poolParent.SetParent(transform);

        foreach (var config in poolConfigs)
        {
            CreatePool(config);
        }
    }

    private void CreatePool(PoolConfig config)
    {
        if (config.prefab == null)
        {
            Debug.LogWarning($"Pool config {config.prefab.name} has null prefab!");
            return;
        }

        var poolableComponent = config.prefab.GetComponent<PoolableComponent>();
        if (poolableComponent == null)
        {
            Debug.LogWarning($"Prefab {config.prefab.name} doesn't have PoolableComponent!");
            return;
        }

        // Создаем контейнер для конкретного пула
        var poolContainer = new GameObject($"Pool_{poolableComponent.GetPoolName()}").transform;
        poolContainer.SetParent(poolParent);

        // Создаем Unity ObjectPool
        var pool = new ObjectPool<PoolableComponent>(
            createFunc: () => CreatePooledObject(config, poolContainer),
            actionOnGet: OnGetFromPool,
            actionOnRelease: OnReturnToPool,
            actionOnDestroy: OnDestroyPooledObject,
            collectionCheck: config.collectionCheck,
            defaultCapacity: config.defaultCapacity,
            maxSize: config.maxSize
        );

        pools.Add(poolableComponent.GetPoolName(), pool);
        configs.Add(poolableComponent.GetPoolName(), config);

        // Предварительно создаем объекты до defaultCapacity
        var preCreatedObjects = new List<PoolableComponent>();
        for (int i = 0; i < config.defaultCapacity; i++)
        {
            preCreatedObjects.Add(pool.Get());
        }

        // Возвращаем обратно в пул
        foreach (var obj in preCreatedObjects)
        {
            pool.Release(obj);
        }
    }


    private PoolableComponent CreatePooledObject(PoolConfig config, Transform parent)
    {
        var instance = Instantiate(config.prefab, parent);
        var poolableComponent = instance.GetComponent<PoolableComponent>();

        // Связываем объект с пулом
        poolableComponent.SetPool(pools[poolableComponent.GetPoolName()]);
        
        // Сохраняем контейнер
        poolableComponent.SetParent(parent);

        // ОДИН РАЗ: применяем настройки авто-возврата из конфигурации
        poolableComponent.ApplyAutoReturnConfig(config.enableAutoReturn, config.autoReturnDelay);

        return poolableComponent;
    }

    private void OnGetFromPool(PoolableComponent obj)
    {
        obj.OnGetFromPool();
    }

    private void OnReturnToPool(PoolableComponent obj)
    {
        obj.OnReturnToPool();
    }

    private void OnDestroyPooledObject(PoolableComponent obj)
    {
        if (obj != null)
        {
            Destroy(obj.gameObject);
        }
    }

    public bool ContainsPool(string poolName)
    {
        return pools.ContainsKey(poolName);
    }

    public T Get<T>(string poolName) where T : PoolableComponent
    {
        if (pools.TryGetValue(poolName, out var pool))
        {
            var obj = pool.Get();
            return obj as T;
        }

        Debug.LogWarning($"Pool '{poolName}' not found!");
        return null;
    }

    public PoolableComponent Get(string poolName)
    {
        return Get<PoolableComponent>(poolName);
    }

    public void Release(string poolName, PoolableComponent obj)
    {
        if (pools.TryGetValue(poolName, out var pool))
        {
            pool.Release(obj);
        }
        else
        {
            Debug.LogWarning($"Pool '{poolName}' not found!");
        }
    }

    public void ClearPool(string poolName)
    {
        if (pools.TryGetValue(poolName, out var pool))
        {
            pool.Clear();
        }
    }

    public void ClearAllPools()
    {
        foreach (var pool in pools.Values)
        {
            pool.Clear();
        }
    }
}

[Serializable]
public class PoolConfig
{
    [Header("Pool Settings")]
    public GameObject prefab;

    [Header("Pool Parameters")] 
    [Tooltip("Create objects on initialization")]
    public bool collectionCheck = true;

    [Tooltip("Initial number of objects")]
    public int defaultCapacity = 10;

    [Tooltip("Maximum number of objects")] 
    public int maxSize = 50;

    [Header("Auto Return")] 
    [Tooltip("Enable auto-return object to pool after time")]
    public bool enableAutoReturn = false;

    [Tooltip("Seconds before auto-return (per Get)")]
    [Min(0f)]
    public float autoReturnDelay = 3f;
}
