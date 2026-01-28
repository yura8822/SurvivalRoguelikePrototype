using UnityEngine;
using UnityEngine.Events;

public class PlayerService : MonoBehaviour
{
    [Header("Player Prefab")]
    [SerializeField] private GameObject playerPrefab;
   
    private PlayerController player;
    
    public GameObject PlayerPrefab => playerPrefab;
    public PlayerController Player => player;
    
    public UnityAction<PlayerController> OnPlayerRegistered;
    public event UnityAction OnPlayerUnregistered;
    
    public bool HasPlayer => player != null;

    public void RegisterPlayer(PlayerController playerController)
    {
        if (player != null)
        {
            Debug.LogWarning("[PlayerService] Player already registered! Overwriting.");
        }

        player = playerController;
        Debug.Log($"[PlayerService] Player registered: {player.name}");
     
        OnPlayerRegistered?.Invoke(player);
    }

    public void UnregisterPlayer()
    {
        if (player == null)
        {
            Debug.LogWarning("[PlayerService] No player to unregister!");
            return;
        }

        Debug.Log($"[PlayerService] Player unregistered: {player.name}");
        player = null;
        
        OnPlayerUnregistered?.Invoke();
    }

    public Vector3 GetPlayerPosition()
    {
        if (player == null)
        {
            Debug.LogWarning("[PlayerService] Player not registered!");
            return Vector3.zero;
        }

        return player.transform.position;
    }

    public Transform GetPlayerTransform()
    {
        if (player == null)
        {
            Debug.LogWarning("[PlayerService] Player not registered!");
            return null;
        }
        
        return player.transform;
    }
}