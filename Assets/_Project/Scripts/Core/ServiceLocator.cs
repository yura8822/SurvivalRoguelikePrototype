using System;
using System.Collections.Generic;
using UnityEngine;

public static class ServiceLocator
{
    private static readonly Dictionary<Type, object> services = new();
    private static bool isShuttingDown = false;

    /// <summary>
    /// Registers a service instance in the locator.
    /// </summary>
    public static void Register<T>(T service) where T : class
    {
        if (isShuttingDown) return;
        
        var type = typeof(T);
        
        if (services.ContainsKey(type))
        {
            Debug.LogWarning($"[ServiceLocator] Service {type.Name} already registered. Overwriting.");
            services[type] = service;
            return;
        }
        
        services.Add(type, service);
        Debug.Log($"[Service] Registered: {type.Name}");
    }

    /// <summary>
    /// Retrieves a registered service instance (returns null if not found or during shutdown).
    /// </summary>
    public static T Get<T>() where T : class
    {
        if (isShuttingDown) return null;

        var type = typeof(T);
        
        if (services.TryGetValue(type, out var service))
        {
            return service as T;
        }

        Debug.Log($"[Service] Service {type.Name} not found!");
        return null;
    }

    /// <summary>
    /// Checks whether a service is registered.
    /// </summary>
    public static bool Has<T>() where T : class
    {
        return !isShuttingDown && services.ContainsKey(typeof(T));
    }

    /// <summary>
    /// Unregisters a service from the locator.
    /// </summary>
    public static void Unregister<T>() where T : class
    {
        var type = typeof(T);
        if (services.Remove(type))
        {
            Debug.Log($"[Service]Unregistered: {type.Name}");
        }
    }

    /// <summary>
    /// Clears all services during application shutdown.
    /// </summary>
    public static void Shutdown()
    {
        isShuttingDown = true;
        services.Clear();
        Debug.Log($"[Service]Shutdown complete.");
    }

    /// <summary>
    /// Resets the locator state (useful for scene transitions).
    /// </summary>
    public static void Reset()
    {
        services.Clear();
        isShuttingDown = false;
    }

    /// <summary>
    /// Automatically resets static state on domain reload.
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        services.Clear();
        isShuttingDown = false;
    }
}
