using UnityEngine;

public interface IPoolable
{
    /// <summary>
    /// Called when popping from the pool
    /// </summary>
    void OnGetFromPool();

    /// <summary>
    /// Called when returning to the poo
    /// </summary>
    void OnReturnToPool();

    /// <summary>
    /// Checking if an object can be returned to the pool
    /// </summary>
    bool CanReturnToPool { get; }
   
    /// <summary>
    /// Returns the pool name identifier
    /// </summary>
    string GetPoolName();
}
