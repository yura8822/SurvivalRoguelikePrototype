using System;
using UnityEngine;

[RequireComponent(typeof(PlayerStats))]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerShootAbility))]
[RequireComponent(typeof(PlayerDamageReceiver))]
public class PlayerController : MonoBehaviour
{
   private PlayerMovement movement;
   private PlayerStats stats;

   private void Awake()
   {
      stats = GetComponent<PlayerStats>();
      movement = GetComponent<PlayerMovement>();
   }
   
   private void OnEnable()
   {
      if (stats != null) stats.OnDeath += OnPlayerDeath;
   }

   private void OnDisable()
   {
      if (stats != null) stats.OnDeath -= OnPlayerDeath;
   }

   private void Start()
   {
     RegisterPlayer();
     InitializePlayer();
   }

   private void OnDestroy()
   {
      UnregisterPlayer();
   }
   
   private void InitializePlayer()
   {
      if (Services.ConfigService == null || !Services.ConfigService.IsLoaded)
      {
         Debug.LogError("[PlayerController] ConfigService not available!");
         return;
      }
      
      PlayerConfig config = Services.ConfigService.GetPlayerConfig();

      if (config == null)
      {
         Debug.LogError("[PlayerController] PlayerConfig not found!");
         return;
      }

      stats?.Initialize(config);
      
      movement?.Initialize(stats);
      
      Debug.Log("[PlayerController] Player initialized from config!");
   }
   
   private void RegisterPlayer()
   {
      if (Services.PlayerService != null) Services.PlayerService.RegisterPlayer(this);
      else Debug.LogError("[PlayerController] PlayerService not found!");
   }

   private void UnregisterPlayer()
   {
      Services.PlayerService?.UnregisterPlayer();
   }
 
   
   private void OnPlayerDeath()
   {
      Debug.Log("[PlayerController] Player died, transitioning to ResultState...");
      movement?.SetEnabled(false);
      
      UnregisterPlayer();
      
      // TODO: Переход в ResultState через StateMachine
      // stateMachine.ChangeState<ResultState>();
      
      gameObject.SetActive(false);
   }
   
   
   
   public void Die()
   {
      Services.PlayerService?.UnregisterPlayer();
   }


   public PlayerStats GetStats() => stats;
   
   public PlayerMovement GetMovement() => movement;
   
   public Vector2 GetMoveDirection() => movement != null ? movement.GetMoveDirection() : Vector2.zero;
   
   public bool IsMoving() => movement != null && movement.IsMoving();

   public bool IsAlive() => stats != null && stats.IsAlive;
}
