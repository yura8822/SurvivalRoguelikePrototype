using UnityEngine;

public class GameBootstrapper : MonoBehaviour
{
    [SerializeField] private PoolService poolService;
    [SerializeField] private ConfigService configService;
    [SerializeField] private PlayerService playerService;
    [SerializeField] private SpatialGridService spatialGridService;

    private void Awake()
    {
        if (!ValidateServices()) return;

        ServiceLocator.Register(poolService);
        ServiceLocator.Register(configService);
        ServiceLocator.Register(playerService);
        ServiceLocator.Register(spatialGridService);
    }
    
    private void OnApplicationQuit()
    {
        ServiceLocator.Shutdown();
    }
    
    private bool ValidateServices()
    {
        bool valid = true;
        
        if (!poolService) { Debug.LogError("[GameBootstrapper] PoolService is NULL!"); valid = false; }
        if (!configService) { Debug.LogError("[GameBootstrapper] ConfigService is NULL!"); valid = false; }
        if  (!playerService) { Debug.LogError("[GameBootstrapper] PlayerService is NULL!"); valid = false; }
        if (!spatialGridService){ Debug.LogError("[GameBootstrapper] SpatialGridService is NULL!"); valid = false; }
        
        return valid;
    }
}

public class Services
{
    public static PoolService PoolService => ServiceLocator.Get<PoolService>();
    public static ConfigService ConfigService => ServiceLocator.Get<ConfigService>();
    public static PlayerService PlayerService => ServiceLocator.Get<PlayerService>();
    public static SpatialGridService SpatialGridService => ServiceLocator.Get<SpatialGridService>();
}

