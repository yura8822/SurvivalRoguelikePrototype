using UnityEngine;
using UnityEngine.Pool;

public abstract class PoolableComponent : MonoBehaviour, IPoolable
{
    private IObjectPool<PoolableComponent> pool;
    private bool isInPool;
    private Transform parent;


    [SerializeField] private bool enableAutoReturn = false;
    [SerializeField] private float autoReturnDelay = 3f;

    // Внутреннее состояние авто-возврата
    private float autoTimer = -1f;
    private bool autoArmed = false;

    /// <summary>
    /// Set a reference to a pool (called on creation)
    /// </summary>
    public void SetPool(IObjectPool<PoolableComponent> pool)
    {
        this.pool = pool;
    }

    public void SetParent(Transform parent)
    {
        this.parent = parent;
    }

    /// <summary>
    /// Optionally set/override auto-return configuration from PoolConfig
    /// </summary>
    public void ApplyAutoReturnConfig(bool enable, float delay)
    {
        enableAutoReturn = enable;
        autoReturnDelay = delay;
    }

    public virtual void OnGetFromPool()
    {
        isInPool = false;
        gameObject.SetActive(true);


        DisarmAutoReturn();
        ArmAutoReturnIfNeeded();
    }

    public virtual void OnReturnToPool()
    {
        isInPool = true;
        gameObject.SetActive(false);
        
        transform.SetParent(parent);

        // Гарантированно снять таймер
        DisarmAutoReturn();
    }

    public virtual bool CanReturnToPool => gameObject.activeInHierarchy;

    public virtual string GetPoolName()
    {
        return gameObject.name.Replace("(Clone)", "").Trim();
    }

    /// <summary>
    /// Auto return to pool upon deactivation (optional)
    /// </summary>
    protected virtual void OnDisable()
    {
        // Если объект деактивировали извне и он ещё не в пуле — вернём его

        if (pool != null && !isInPool && CanReturnToPool)
        {
            // Важно отключить таймер до Release, чтобы исключить двойной возврат
            DisarmAutoReturn();
            pool.Release(this);
        }
    }

    private void Update()
    {
        if (!autoArmed) return;

        autoTimer -= Time.deltaTime;
        if (autoTimer <= 0f)
        {
            DisarmAutoReturn();

            if (pool != null && !isInPool && CanReturnToPool)
            {
                pool.Release(this);
            }
        }
    }

    private void ArmAutoReturnIfNeeded()
    {
        if (!enableAutoReturn) return;
        autoTimer = Mathf.Max(0f, autoReturnDelay);
        autoArmed = true;
    }

    private void DisarmAutoReturn()
    {
        autoArmed = false;
        autoTimer = -1f;
    }
}